using EstudiantesApi.Dtos;
using FluentValidation;

namespace EstudiantesApi.Validators
{
    public class CreateEstudianteValidator : AbstractValidator<CreateEstudianteDto>
    {
        public CreateEstudianteValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es obligatorio");
            RuleFor(x => x.Apellido).NotEmpty().WithMessage("El apellido es obligatorio");
            RuleFor(x => x.FechaNacimiento).LessThan(DateTime.UtcNow).WithMessage("La fecha de nacimiento debe ser pasada");
        }
    }
}
