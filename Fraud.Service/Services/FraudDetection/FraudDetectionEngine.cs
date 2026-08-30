using Fraud.Core.Entities;
using Fraud.Core.Enum;
using Fraud.Core.FraudDetection.Abstractions;
using Fraud.Core.FraudDetection.Models;
using Fraud.Core.FraudDetection.Options;
using Fraud.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fraud.Service.Services.FraudDetection
{
    public sealed class FraudDetectionEngine : IFraudDetectionEngine
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEnumerable<IFraudRule> _rules;
        private readonly IOptionsMonitor<FraudDetectionOptions> _options;
        private readonly IRiskSeverityClassifier _severityClassifier;
        private readonly ILogger<FraudDetectionEngine> _logger;

        public FraudDetectionEngine(
            ITransactionRepository transactionRepository,
            IEnumerable<IFraudRule> rules,
            IOptionsMonitor<FraudDetectionOptions> options,
            IRiskSeverityClassifier severityClassifier,
            ILogger<FraudDetectionEngine> logger)
        {
            _transactionRepository = transactionRepository;
            _rules = rules;
            _options = options;
            _severityClassifier = severityClassifier;
            _logger = logger;
        }

        public async Task<FraudEvaluationResult> EvaluateAsync(Transaction transaction, CancellationToken ct = default)
        {
            var options = _options.CurrentValue;

            if (!options.Enabled)
            {
                return new FraudEvaluationResult
                {
                    RiskScore = 0,
                    Severity = FraudSeverity.Low,
                    Approved = true,
                    RequiresFraudCase = false,
                    FailureReason = null,
                    RuleResults = Array.Empty<FraudRuleResult>()


                };
            }

            if (transaction.FromCard is null || transaction.ToCard is null)
                throw new InvalidOperationException(
                    "Transaction.FromCard and Transaction.ToCard must be populated before fraud evaluation.");

            var context = await BuildContextAsync(transaction, options, ct);

            // Rule-lar yalnız in-memory context oxuyur (DbContext-ə toxunmur), ona görə paralel
            // işlədilə bilər. Əgər gələcəkdə bir rule DB-yə birbaşa müraciət etməli olsa,
            // MÜTLƏQ sequential icraya keçilməlidir (DbContext thread-safe deyil).
            var ruleResults = await Task.WhenAll(_rules.Select(rule => SafeEvaluateAsync(rule, context, ct)));

            var riskScore = RiskScoreAggregator.Aggregate(ruleResults);
            var severity = _severityClassifier.Classify(riskScore);
            var approved = severity < options.Decision.BlockAtOrAbove;
            var requiresFraudCase = severity >= options.Decision.CreateFraudCaseAtOrAbove;

            string? failureReason = null;
            if (!approved)
            {
                failureReason = string.Join(" | ", ruleResults
                    .Where(r => r.Triggered && r.Reason is not null)
                    .Select(r => $"[{r.RuleName}] {r.Reason}"));
            }

            return new FraudEvaluationResult
            {
                RiskScore = riskScore,
                Severity = severity,
                Approved = approved,
                RequiresFraudCase = requiresFraudCase,
                FailureReason = failureReason,
                RuleResults = ruleResults
            };
        }

        private async Task<FraudRuleResult> SafeEvaluateAsync(
            IFraudRule rule, FraudEvaluationContext context, CancellationToken ct)
        {
            try
            {
                return await rule.EvaluateAsync(context, ct);
            }
            catch (Exception ex)
            {
                // Fail-open by design: bir rule-dakı bug bütün ödəniş axınını bloklamamalıdır.
                // Xəta loglanır, rule bu evaluation üçün 0 risk verir.
                _logger.LogError(ex, "Fraud rule {RuleName} threw an exception during evaluation.", rule.RuleName);
                return FraudRuleResult.NotTriggered(rule.RuleName);
            }
        }

        private async Task<FraudEvaluationContext> BuildContextAsync(
            Transaction transaction, FraudDetectionOptions options, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var senderUserId = transaction.FromCard.UserId;
            var recipientUserId = transaction.ToCard.UserId;

            var historyLookback = now.AddDays(-options.HistoryLookbackDays);

            var shortWindowMinutes = new[]
            {
            options.Velocity.Windows.Count > 0 ? options.Velocity.Windows.Max(w => w.TimeWindowMinutes) : 0,
            options.MultipleRecipients.TimeWindowMinutes,
            options.FailedTransactionPattern.TimeWindowMinutes,
            options.BehavioralAnomaly.FrequencyWindowMinutes
        }.Max();

            var shortWindowStart = now.AddMinutes(-Math.Max(shortWindowMinutes, 1));

            // Eyni scoped DbContext üzərində ardıcıl await olunur (paralel EF sorğusu icazəli deyil).
            var approvedHistory = await _transactionRepository.GetApprovedOutgoingHistoryAsync(
                senderUserId, historyLookback, now, options.HistoryMaxRecords, ct);

            var recentWindowTransactions = await _transactionRepository.GetOutgoingTransactionsInWindowAsync(
                senderUserId, shortWindowStart, now, ct);

            var recipientHasPriorApproved = transaction.IsSelfTransfer
                || await _transactionRepository.HasPriorApprovedTransactionToRecipientAsync(
                    senderUserId, recipientUserId, now, ct);

            var todayStart = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            var todayApproved = approvedHistory.Where(h => h.CreateDate >= todayStart).ToList();

            return new FraudEvaluationContext
            {
                Transaction = transaction,
                FromCard = transaction.FromCard,
                ToCard = transaction.ToCard,
                SenderUserId = senderUserId,
                RecipientUserId = recipientUserId,
                UtcNow = now,
                ApprovedOutgoingHistory = approvedHistory,
                RecentOutgoingTransactions = recentWindowTransactions,
                TodayApprovedOutgoingTransactions = todayApproved,
                RecipientHasPriorApprovedTransaction = recipientHasPriorApproved
            };
        }
    }
}
