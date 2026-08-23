using FluentValidation;
using Fraud.Core.Entities;
using Fraud.DTO.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service.Validators
{
    public class CreateCardDtoValidator : AbstractValidator<CreateCardDto>
    {
        public CreateCardDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

            RuleFor(x => x.Code)
                .GreaterThan(0).WithMessage("Code must be a valid positive number");

            RuleFor(x => x.ValidDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("ValidDate cannot be in the past");
        }
    }
}
