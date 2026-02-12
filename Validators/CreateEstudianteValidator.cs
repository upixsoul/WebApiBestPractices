using EstudiantesApi.Dtos;
using FluentValidation;

namespace EstudiantesApi.Validators
{
    public class CreateEstudianteValidator : AbstractValidator<CreateEstudianteDto>
    {
        public CreateEstudianteValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(1000).WithMessage("El nombre no puede exceder los 1000 caracteres");

            RuleFor(x => x.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio")
                .MaximumLength(1000).WithMessage("El apellido no puede exceder los 1000 caracteres");

            RuleFor(x => x.FechaNacimiento)
                .LessThan(DateTime.UtcNow).WithMessage("La fecha de nacimiento debe ser una fecha pasada");
        }
    }
}
