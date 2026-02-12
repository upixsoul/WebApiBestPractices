using EstudiantesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EstudiantesApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base method if inheriting from a base context (like IdentityDbContext)
            base.OnModelCreating(modelBuilder);

            //error to test
            //throw new Exception("Test Error");

            modelBuilder.Entity<Qualification>()
                .HasOne(c => c.Student)
                .WithMany(e => e.Qualifications)
                .HasForeignKey(c => c.StudentId);
        }
    }
}
