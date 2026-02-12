using EstudiantesApi.Dtos;
using FluentValidation;

namespace EstudiantesApi.Validators
{
    public class CreateQualificationValidator : AbstractValidator<CreateQualificationDto>
    {
        public CreateQualificationValidator()
        {
            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("The subject is required")
                .MaximumLength(1000).WithMessage("The subject cannot exceed 1000 characters")
                .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("The subject can only contain letters, numbers, spaces, and hyphens");

            RuleFor(x => x.Grade)
                .GreaterThan(0m).WithMessage("The grade cannot be zero or negative")
                .InclusiveBetween(1.0m, 10.0m).WithMessage("The grade must be between 1.0 and 10.0")
                .PrecisionScale(4, 1, false).WithMessage("The grade must have at most 1 decimal place (e.g., 9.5)");
        }
    }
}
