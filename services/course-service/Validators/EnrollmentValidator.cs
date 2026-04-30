using FluentValidation;
using Courses.Models.DTOs;

namespace Courses.Api.Validators
{
    public class EnrollmentValidator : AbstractValidator<EnrollmentCreateDTO>
    {
        public EnrollmentValidator()
        {
            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Course ID must be greater than 0.");
        }
    }
}
