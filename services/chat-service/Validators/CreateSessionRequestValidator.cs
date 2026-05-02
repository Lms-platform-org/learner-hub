using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validators
{
    public class CreateSessionRequestValidator : AbstractValidator<CreateSessionRequest>
    {
        public CreateSessionRequestValidator()
        {
            RuleFor(x => x.StudentId).NotEmpty().WithMessage("StudentId is required");
            RuleFor(x => x.TeacherId).NotEmpty().WithMessage("TeacherId is required");
        }
    }
}