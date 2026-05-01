using FluentValidation;
using TeacherDashboardApi.DTOs;

namespace TeacherDashboardApi.Validators
{
    public class TeacherProfileValidator : AbstractValidator<TeacherProfileDTO>
    {
        public TeacherProfileValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.");

            RuleFor(x => x.PreferredLevel)
                .NotEmpty().WithMessage("Preferred Teaching Level is required.");
        }
    }
}
