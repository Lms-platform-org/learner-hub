using FluentValidation;
using TeacherDashboardApi.DTOs;

namespace TeacherDashboardApi.Validators
{
    public class SendMessageValidator : AbstractValidator<SendMessageDto>
    {
        public SendMessageValidator()
        {
            RuleFor(x => x.ChatSessionId)
                .GreaterThan(0).WithMessage("Valid Chat Session ID is required.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Message content cannot be empty.")
                .MaximumLength(2000).WithMessage("Message content cannot exceed 2000 characters.");

            RuleFor(x => x.SenderRole)
                .NotEmpty().WithMessage("Sender role is required.")
                .Must(role => role == "Teacher").WithMessage("Sender role must be 'Teacher'.");
        }
    }
}
