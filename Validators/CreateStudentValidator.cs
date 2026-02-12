using StudentsApi.Dtos;
using FluentValidation;

namespace StudentsApi.Validators
{
    public class CreateStudentValidator : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("The first name is required")
                .MaximumLength(1000).WithMessage("The first name cannot exceed 1000 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("The last name is required")
                .MaximumLength(1000).WithMessage("The last name cannot exceed 1000 characters");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow).WithMessage("The date of birth must be in the past");
        }
    }
}
