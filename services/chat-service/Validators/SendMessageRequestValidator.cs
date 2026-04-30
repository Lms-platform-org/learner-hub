using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validators
{
    public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
    {
        public SendMessageRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .Must(content => (content ?? string.Empty)
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Length >= 3)
                .WithMessage("Message must contain at least 3 words.");

            RuleFor(x => x.ChatSessionId)
                .GreaterThan(0);

            RuleFor(x => x.SenderRole)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}