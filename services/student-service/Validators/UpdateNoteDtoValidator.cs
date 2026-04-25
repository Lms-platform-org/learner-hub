using FluentValidation;
using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Validators
{
    public class UpdateNoteDtoValidator : AbstractValidator<UpdateNoteDto>
    {
        public UpdateNoteDtoValidator()
        {
            RuleFor(x => x.Note)
                .MaximumLength(2000).WithMessage("Note cannot exceed 2000 characters");
        }
    }
}
