using ChatApp.Application.DTOs.Auth;
using FluentValidation;

namespace ChatApp.Application.Validators
{
    /// <summary>
    /// Validates login requests.
    /// </summary>
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
