using AutoMapper;
using FluentValidation;
using Fraud.Core.Entities;
using Fraud.Core.Enum;
using Fraud.Core.Exceptions;
using Fraud.Core.FraudDetection.Abstractions;
using Fraud.Core.Interfaces;
using Fraud.DataAccess.Repositories;
using Fraud.DTO.Auth;
using Fraud.DTO.Card;
using Fraud.DTO.Transaction;
using Fraud.Service.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fraud.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transationsRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IValidator<CreateTransactionDto> _createValidator;
        private readonly IMapper _mapper;
        private readonly IFraudDetectionEngine _fraudEngine;
        private readonly IFraudCaseFactory _fraudCaseFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public TransactionService(
            ITransactionRepository transationsRepository,
            ICardRepository cardRepository,
            IValidator<CreateTransactionDto> createValidator,
            IMapper mapper,
            IFraudDetectionEngine fraudEngine,
            IHttpContextAccessor httpContextAccessor,
            IFraudCaseFactory fraudCaseFactory)
        {
            _transationsRepository = transationsRepository;
            _cardRepository        = cardRepository;
            _createValidator       = createValidator;
            _mapper                = mapper;
            _fraudEngine           = fraudEngine;
            _fraudCaseFactory      = fraudCaseFactory;
            _httpContextAccessor   = httpContextAccessor;
        }

        public async Task<TransactionResponseDto> CreateTransactionAsync(CreateTransactionDto dto,CancellationToken ct = default)
        {
            await using var dbTransaction =
                await _transationsRepository.BeginTransactionAsync(ct);

            var currentUserId = GetCurrentUserId();

            try
            {
                var fromCard = await _cardRepository
                    .GetCardForUpdateAsync(dto.FromCardId, ct)
                    ?? throw new CardNotFoundException(dto.FromCardId);

                var toCard = await _cardRepository
                    .GetCardForUpdateAsync(dto.ToCardId, ct)
                    ?? throw new CardNotFoundException(dto.ToCardId);

                if (fromCard.UserId != currentUserId)
                    throw new Fraud.Core.Exceptions.UnauthorizedAccessException();

                var validationResult =
                    await _createValidator.ValidateAsync(dto, ct);

                if (!validationResult.IsValid)
                    throw new Fraud.Core.Exceptions.ValidationException(
                        validationResult.Errors.Select(e => e.ErrorMessage));

                var transaction = new Transaction
                {
                    FromCardId = fromCard.Id,
                    FromCard = fromCard,           // in-memory fixup: fraud engine üçün, əlavə query yaratmır
                    ToCardId = toCard.Id,
                    ToCard = toCard,                // in-memory fixup
                    Amount = dto.Amount,
                    Type = dto.Type,
                    Status = TransactionStatus.Pending,
                    IsSelfTransfer = fromCard.UserId == toCard.UserId,
                };

                var evaluation =
                    await _fraudEngine.EvaluateAsync(transaction, ct);

                transaction.RiskScore = evaluation.RiskScore;
                transaction.RiskLevel = evaluation.Severity;
                transaction.FraudEvaluatedAt = DateTime.UtcNow;

                if (evaluation.Approved)
                {
                    if (fromCard.Balance < dto.Amount)
                        throw new InsufficientBalanceException(fromCard.Id);

                    fromCard.Balance -= dto.Amount;
                    toCard.Balance += dto.Amount;

                    transaction.Status = TransactionStatus.Approved;
                    transaction.CompletedAt = DateTime.UtcNow;
                }
                else
                {
                    transaction.Status = TransactionStatus.Blocked;
                    transaction.FailureReason = evaluation.FailureReason;

                    if (evaluation.RequiresFraudCase)
                    {
                        var fraudCase = _fraudCaseFactory.Create(evaluation, transaction);
                        transaction.FraudCase = fraudCase;
                    }
                }

                await _transationsRepository.AddAsync(transaction, ct);
                await _transationsRepository.SaveChangesAsync(ct);

                await dbTransaction.CommitAsync(ct);

                return _mapper.Map<TransactionResponseDto>(transaction);
            }
            catch
            {
                await dbTransaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<List<TransactionResponseDto>> GetByCardIdAsync(int cardId, CancellationToken ct = default)
        {

            var currentUserId = GetCurrentUserId();

            var card = await _cardRepository.GetByIdAsync(cardId, ct)
                ?? throw new CardNotFoundException(cardId);

            if (card.UserId != currentUserId)
                throw new Core.Exceptions.UnauthorizedAccessException();

            var transactions = await _transationsRepository.GetByCardIdAsync(cardId, ct);

            return _mapper.Map<List<TransactionResponseDto>>(transactions);
        }

        public async Task<TransactionResponseDto?> GetByIdAsync(int id,CancellationToken ct = default)
        {
            var currentUserId = GetCurrentUserId();

            var transaction = await _transationsRepository.GetByIdAsync(id, ct);

            if (transaction is null)
                return null;

            var fromCard = await _cardRepository.GetByIdAsync(transaction.FromCardId,ct);

            var toCard = await _cardRepository.GetByIdAsync(transaction.ToCardId, ct);

            if (fromCard?.UserId != currentUserId &&
                toCard?.UserId != currentUserId)
            {
                throw new Core.Exceptions.UnauthorizedAccessException();
            }

            return _mapper.Map<TransactionResponseDto>(transaction);
        }

        private int GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Core.Exceptions.UnauthorizedAccessException("User not found");

            return int.Parse(userId);
        }

    }
}
