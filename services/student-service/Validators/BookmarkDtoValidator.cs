using FluentValidation;
using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Validators
{
    public class BookmarkDtoValidator : AbstractValidator<BookmarkDto>
    {
        public BookmarkDtoValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required")
                .Must(x => x == "course" || x == "book").WithMessage("Type must be 'course' or 'book'");

            When(x => x.Type == "course", () =>
            {
                RuleFor(x => x.CourseId)
                    .NotNull().WithMessage("CourseId is required for course bookmarks")
                    .GreaterThan(0).WithMessage("CourseId must be greater than 0");
            });

            When(x => x.Type == "book", () =>
            {
                RuleFor(x => x.BookKey)
                    .NotEmpty().WithMessage("BookKey is required for book bookmarks");

                RuleFor(x => x.BookTitle)
                    .NotEmpty().WithMessage("BookTitle is required for book bookmarks")
                    .MaximumLength(500).WithMessage("BookTitle cannot exceed 500 characters");

                RuleFor(x => x.BookAuthor)
                    .MaximumLength(200).WithMessage("BookAuthor cannot exceed 200 characters");
            });

            RuleFor(x => x.PersonalNote)
                .MaximumLength(2000).WithMessage("PersonalNote cannot exceed 2000 characters");

            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");
        }
    }
}
