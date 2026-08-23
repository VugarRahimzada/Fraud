using AutoMapper;
using FluentValidation;
using Fraud.Core.Common;
using Fraud.Core.Entities;
using Fraud.Core.Exceptions;
using Fraud.Core.Interfaces;
using Fraud.DTO.Card;
using Fraud.Service.Interfaces;
using System.Linq.Expressions;

namespace Fraud.Service
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCardDto> _createValidator;
        private readonly IValidator<UpdateCardDto> _updateValidator;

        public CardService(
            ICardRepository cardRepository,
            IMapper mapper,
            IValidator<CreateCardDto> createValidator,
            IValidator<UpdateCardDto> updateValidator)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<PagedResult<CardDto>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
        {
            Expression<Func<Card, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(paginationParams.SearchTerm))
            {
                var term = paginationParams.SearchTerm.Trim();
                filter = x => x.Name.Contains(term);
            }

            var pagedCards = await _cardRepository.GetPagedAsync(paginationParams, filter, cancellationToken);
            return _mapper.Map<PagedResult<CardDto>>(pagedCards);
        }

        public async Task<CardDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var card = await _cardRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Card), id);

            return _mapper.Map<CardDto>(card);
        }

        public async Task<CardDto> CreateAsync(CreateCardDto dto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                throw new Fraud.Core.Exceptions.ValidationException(validationResult.Errors.Select(e => e.ErrorMessage));

            if (await _cardRepository.CodeExistsAsync(dto.Code, cancellationToken: cancellationToken))
                throw new ConflictException($"A card with code '{dto.Code}' already exists.");

            var card = _mapper.Map<Card>(dto);
            await _cardRepository.AddAsync(card, cancellationToken);
            await _cardRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CardDto>(card);
        }

        public async Task<CardDto> UpdateAsync(int id, UpdateCardDto dto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
                throw new Fraud.Core.Exceptions.ValidationException(validationResult.Errors.Select(e => e.ErrorMessage));

            var card = await _cardRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Card), id);

            if (await _cardRepository.CodeExistsAsync(dto.Code, id, cancellationToken))
                throw new ConflictException($"A card with code '{dto.Code}' already exists.");

            _mapper.Map(dto, card);
            _cardRepository.Update(card);
            await _cardRepository.SaveChangesAsync(cancellationToken);

            return _mapper.Map<CardDto>(card);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var card = await _cardRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Card), id);

            _cardRepository.Delete(card);
            await _cardRepository.SaveChangesAsync(cancellationToken);
        }
    }
}