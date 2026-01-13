using ChatApp.Application.DTOs.Auth;
using FluentValidation;

namespace ChatApp.Application.Validators
{
    /// <summary>
    /// Validates password change requests.
    /// </summary>
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}
