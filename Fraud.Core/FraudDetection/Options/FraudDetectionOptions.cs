using Fraud.Core.Entities;
using Fraud.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Core.FraudDetection.Options
{
    public sealed class FraudDetectionOptions
    {
        public const string SectionName = "FraudDetection";

        public bool Enabled { get; set; } = true;

        /// <summary>Ümumi (uzunmüddətli) tarixçə sorğusunun bounded olması üçün.</summary>
        public int HistoryLookbackDays { get; set; } = 90;

        /// <summary>Bir sorğuda maksimum neçə tarixi transaction yüklənsin.</summary>
        public int HistoryMaxRecords { get; set; } = 200;

        public RiskThresholdsOptions RiskThresholds { get; set; } = new();
        public DecisionOptions Decision { get; set; } = new();

        public LargeAmountOptions LargeAmount { get; set; } = new();
        public AmountDeviationOptions AmountDeviation { get; set; } = new();
        public VelocityOptions Velocity { get; set; } = new();
        public DailyAmountOptions DailyAmount { get; set; } = new();
        public NewRecipientOptions NewRecipient { get; set; } = new();
        public MultipleRecipientsOptions MultipleRecipients { get; set; } = new();
        public BehavioralAnomalyOptions BehavioralAnomaly { get; set; } = new();
        public FailedTransactionPatternOptions FailedTransactionPattern { get; set; } = new();
    }

    public sealed class RiskThresholdsOptions
    {
        public decimal Medium { get; set; } = 30;
        public decimal High { get; set; } = 60;
        public decimal Critical { get; set; } = 80;
    }

    public sealed class DecisionOptions
    {
        public FraudSeverity BlockAtOrAbove { get; set; } = FraudSeverity.High;
        public FraudSeverity CreateFraudCaseAtOrAbove { get; set; } = FraudSeverity.High;
    }

    public sealed class LargeAmountOptions
    {
        public decimal Threshold { get; set; } = 10000m;
        public decimal RiskScore { get; set; } = 40m;

        /// <summary>Self-transfer-də bu rule tam ignore olunmur, amma çəkisi azaldılır.</summary>
        public decimal SelfTransferRiskMultiplier { get; set; } = 0.3m;
    }

    public sealed class AmountDeviationOptions
    {
        public int MinimumHistoryCount { get; set; } = 5;
        public decimal StandardDeviationMultiplier { get; set; } = 3m;
        public decimal RiskScore { get; set; } = 30m;
    }

    public sealed class VelocityOptions
    {
        public List<VelocityWindowOptions> Windows { get; set; } = new();
    }

    public sealed class VelocityWindowOptions
    {
        public int TimeWindowMinutes { get; set; }
        public int MaxTransactionCount { get; set; }
        public decimal RiskScore { get; set; }
    }

    public sealed class DailyAmountOptions
    {
        public decimal DailyLimit { get; set; } = 20000m;
        public decimal RiskScore { get; set; } = 25m;
    }

    public sealed class NewRecipientOptions
    {
        public decimal MinimumAmount { get; set; } = 1000m;
        public int MinimumSenderHistoryCount { get; set; } = 3;
        public decimal RiskScore { get; set; } = 20m;

        /// <summary>Yeni recipient + böyük məbləğ kombinasiyası üçün risk artımı.</summary>
        public decimal LargeAmountThreshold { get; set; } = 5000m;
        public decimal LargeAmountMultiplier { get; set; } = 1.5m;
    }

    public sealed class MultipleRecipientsOptions
    {
        public int TimeWindowMinutes { get; set; } = 10;
        public int MaxDistinctRecipients { get; set; } = 5;
        public decimal RiskScore { get; set; } = 35m;
    }

    public sealed class BehavioralAnomalyOptions
    {
        public int MinimumHistoryCount { get; set; } = 10;

        public decimal UnusualHourRiskScore { get; set; } = 15m;
        public decimal UnusualAmountRiskScore { get; set; } = 20m;
        public decimal UnusualFrequencyRiskScore { get; set; } = 20m;

        public decimal AmountStandardDeviationMultiplier { get; set; } = 2.5m;

        public int FrequencyWindowMinutes { get; set; } = 60;
        public decimal FrequencyDeviationMultiplier { get; set; } = 3m;
    }

    public sealed class FailedTransactionPatternOptions
    {
        public int TimeWindowMinutes { get; set; } = 10;
        public int MaxFailedTransactions { get; set; } = 3;
        public decimal RiskScore { get; set; } = 40m;
    }
}
