using FluentValidation;
using Fraud.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.Service.Validators
{
    public class LoginDtoValidators : AbstractValidator<LoginRequestDto>
    {
        public LoginDtoValidators()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }

    }
}
