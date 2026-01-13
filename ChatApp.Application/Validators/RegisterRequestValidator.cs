using ChatApp.Application.DTOs.Auth;
using FluentValidation;

namespace ChatApp.Application.Validators
{
    /// <summary>
    /// Validates user registration requests.
    /// </summary>
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MinimumLength(3);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}
