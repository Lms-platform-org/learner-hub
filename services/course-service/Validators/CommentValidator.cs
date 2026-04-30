using FluentValidation;
using Courses.Models.DTOs;

namespace Courses.Api.Validators
{
    public class CreateCommentValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Valid Course ID is required.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content cannot be empty.")
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
        }
    }
}
