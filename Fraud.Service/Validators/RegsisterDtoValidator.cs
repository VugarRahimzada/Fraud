using FluentValidation;
using Fraud.DTO.Auth;
using Fraud.DTO.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service.Validators
{
    public class RegsisterDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegsisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(150)
                .WithMessage("Name must not exceed 150 characters");       

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Surname is required")
                .MaximumLength(150)
                .WithMessage("Name must not exceed 150 characters");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters");
        }
    }
}
