using System.ComponentModel.DataAnnotations;

namespace StudentsApi.Models
{
    public class Student
    {
        public int Id { get; set; }
        [MaxLength(1000)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        // Relation 1:N
        public ICollection<Qualification> Qualifications { get; set; } = new List<Qualification>();
    }
}
