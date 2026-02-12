using EstudiantesApi.Dtos;
using FluentValidation;

namespace EstudiantesApi.Validators
{
    public class CreateCalificacionValidator : AbstractValidator<CreateCalificacionDto>
    {
        public CreateCalificacionValidator()
        {
            RuleFor(x => x.Materia)
                .NotEmpty().WithMessage("La materia es obligatoria")
                .MaximumLength(1000).WithMessage("La materia no puede exceder los 1000 caracteres")
                .Matches(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\-']+$").WithMessage("La materia solo puede contener letras, números, espacios, guiones y apóstrofes");

            RuleFor(x => x.Nota)
                .InclusiveBetween(0.0m, 10.0m).WithMessage("La nota debe estar entre 0.0 y 10.0")
                .PrecisionScale(4, 1, false).WithMessage("La nota debe tener máximo 1 decimal (ej: 9.5)");
        }
    }
}
