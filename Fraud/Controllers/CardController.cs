using Fraud.Core.Common;
using Fraud.DTO.Card;
using Fraud.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fraud.Controllers.Controllers
{

    [Authorize]
    [Route("api/cards")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
        {
            var result = await _cardService.GetAllAsync(paginationParams, cancellationToken);
            return Ok(ApiResponse<PagedResult<CardDto>>.SuccessResponse(result, "Cards retrieved successfully"));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var card = await _cardService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<CardDto>.SuccessResponse(card, "Card retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCardDto dto, CancellationToken cancellationToken)
        {
            var created = await _cardService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                ApiResponse<CardDto>.SuccessResponse(created, "Card created successfully"));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCardDto dto, CancellationToken cancellationToken)
        {
            var updated = await _cardService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<CardDto>.SuccessResponse(updated, "Card updated successfully"));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _cardService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse.SuccessResponse("Card deleted successfully"));
        }
    }
}