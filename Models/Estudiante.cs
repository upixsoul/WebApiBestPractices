namespace EstudiantesApi.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }

        // Relación 1:N
        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    }
}
