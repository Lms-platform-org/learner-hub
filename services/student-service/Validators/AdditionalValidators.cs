using FluentValidation;
using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Validators
{
    /// <summary>
    /// Validator for Profile DTO
    /// Example of validating user profile information
    /// </summary>
    public class ProfileDtoValidator : AbstractValidator<ProfileDto>
    {
        public ProfileDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

            RuleFor(x => x.Bio)
                .MaximumLength(300).WithMessage("Bio cannot exceed 300 characters");

            RuleFor(x => x.PreferredLevel)
                .NotEmpty().WithMessage("Preferred level is required")
                .Must(x => x == "Beginner" || x == "Intermediate" || x == "Advanced")
                .WithMessage("Preferred level must be Beginner, Intermediate, or Advanced");

            RuleFor(x => x.Skills)
                .NotNull().WithMessage("Skills list is required")
                .Must(x => x.Count <= 20).WithMessage("Cannot have more than 20 skills");
        }
    }

    /// <summary>
    /// Validator for Review DTO
    /// Example of validating course/book reviews
    /// </summary>
    public class ReviewDtoValidator : AbstractValidator<ReviewDto>
    {
        public ReviewDtoValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required")
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be greater than 0");

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required");

            RuleFor(x => x.StudentName)
                .NotEmpty().WithMessage("StudentName is required")
                .MaximumLength(100).WithMessage("StudentName cannot exceed 100 characters");
        }
    }
}
