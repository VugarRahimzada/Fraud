using FluentValidation;
using Fraud.Core.Entities;
using Fraud.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service.Validators
{
    public class CreateTransactionValidator : AbstractValidator<CreateTransactionDto>
    {
        public CreateTransactionValidator()
        {
            RuleFor(x => x.FromCardId)
                .NotEmpty();

            RuleFor(x => x.ToCardId)
                .NotEmpty();

            RuleFor(x => x)
                .Must(x => x.FromCardId != x.ToCardId)
                .WithMessage("A card cannot transfer to itself.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Type)
                .IsInEnum();
        }
    }
}
