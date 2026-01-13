using ChatApp.Application.DTOs.Chat;
using FluentValidation;

namespace ChatApp.Application.Validators
{
    /// <summary>
    /// Validates send message requests.
    /// </summary>
    public class SendMessageRequestValidator : AbstractValidator<SendMessageRequestDto>
    {
        public SendMessageRequestValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(2000);
        }
    }
}
