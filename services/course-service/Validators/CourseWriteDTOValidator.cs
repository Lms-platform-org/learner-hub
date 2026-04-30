using FluentValidation;
using Courses.Models.DTOs;

namespace Courses.Api.Validators
{
    public class CourseWriteDTOValidator : AbstractValidator<CourseWriteDTO>
    {
        public CourseWriteDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.Duration)
                .NotEmpty().WithMessage("Duration is required.");

            RuleFor(x => x.ThumbnailUrl)
                .MaximumLength(500).WithMessage("Thumbnail URL cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl));

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .Must(date => date >= DateTime.UtcNow.Date).WithMessage("Start date cannot be in the past.");
        }
    }
}
