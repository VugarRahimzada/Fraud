using Fraud.Core.Common;
using Fraud.Core.Entities;
using Fraud.Core.Exceptions;
using Fraud.DTO.Card;
using Fraud.DTO.Transaction;
using Fraud.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Fraud.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponseDto>> Create([FromBody] CreateTransactionDto dto, CancellationToken ct)
        {
            try
            {
                var result = await _transactionService.CreateTransactionAsync(dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (CardNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InsufficientBalanceException ex)
            {
                return UnprocessableEntity(new { error = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TransactionResponseDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _transactionService.GetByIdAsync(id, ct);

            return Ok(ApiResponse<TransactionResponseDto>.SuccessResponse(result, "Successfully"));

        }

        [HttpGet("by-card/{cardId:int}")]
        public async Task<ActionResult<List<TransactionResponseDto>>> GetByCard(int cardId, CancellationToken ct)
        {
            var results = await _transactionService.GetByCardIdAsync(cardId, ct);
            return Ok(ApiResponse<List<TransactionResponseDto>>.SuccessResponse(results, "Successfully"));

        }
    }
}
