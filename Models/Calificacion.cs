namespace EstudiantesApi.Models
{
    public class Calificacion
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public string Materia { get; set; } = string.Empty;
        public decimal Nota { get; set; }   // 0.0 - 10.0
        public DateTime Fecha { get; set; }

        public Estudiante Estudiante { get; set; } = null!;
    }
}
