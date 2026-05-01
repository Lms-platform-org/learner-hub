using FluentValidation;
using TeacherDashboardApi.DTOs;

namespace TeacherDashboardApi.Validators
{
    public class CourseCreateValidator : AbstractValidator<CourseCreateDTO>
    {
        public CourseCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Course title is required.")
                .MaximumLength(100).WithMessage("Course title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Course description is required.")
                .MinimumLength(10).WithMessage("Course description must be at least 10 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.Duration)
                .NotEmpty().WithMessage("Duration is required.");
        }
    }

    public class CourseUpdateValidator : AbstractValidator<CourseUpdateDTO>
    {
        public CourseUpdateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Course title is required.")
                .MaximumLength(100).WithMessage("Course title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Course description is required.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.");
        }
    }
}
