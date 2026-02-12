using System.ComponentModel.DataAnnotations;

namespace EstudiantesApi.Models
{
    public class Qualification
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        [MaxLength(1000)]
        public string Subject { get; set; } = string.Empty;
        public decimal Grade { get; set; }   // 0.0 - 10.0
        public DateTime Date { get; set; }

        public Student Student { get; set; } = null!;
    }
}
