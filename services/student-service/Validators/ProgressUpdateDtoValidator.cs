using FluentValidation;
using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Validators
{
    public class ProgressUpdateDtoValidator : AbstractValidator<ProgressUpdateDto>
    {
        public ProgressUpdateDtoValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be greater than 0");

            RuleFor(x => x.Percentage)
                .InclusiveBetween(0, 100).WithMessage("Percentage must be between 0 and 100");
        }
    }
}
