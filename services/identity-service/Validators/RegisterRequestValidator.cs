using FluentValidation;
using LearningPlatformAuth.Models;

namespace LearningPlatformAuth.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(r => r == "Student" || r == "Teacher")
                .WithMessage("Role must be either 'Student' or 'Teacher'.");
        }
    }
}
