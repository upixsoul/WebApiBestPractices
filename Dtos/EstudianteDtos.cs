namespace EstudiantesApi.Dtos
{
    public record CreateEstudianteDto(string Nombre, string Apellido, DateTime FechaNacimiento);
    public record EstudianteResponseDto(int Id, string Nombre, string Apellido, DateTime FechaNacimiento);

    public record CreateCalificacionDto(string Materia, decimal Nota);
    public record CalificacionResponseDto(int Id, string Materia, decimal Nota, DateTime Fecha);
}
