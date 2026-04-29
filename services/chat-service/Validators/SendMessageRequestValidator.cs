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
                .MinimumLength(5);

            RuleFor(x => x.ChatSessionId)
                .GreaterThan(0);
        }
    }
}