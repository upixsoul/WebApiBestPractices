using EstudiantesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EstudiantesApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base method if inheriting from a base context (like IdentityDbContext)
            base.OnModelCreating(modelBuilder);

            //error to test
            //throw new Exception("Test Error");

            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Estudiante)
                .WithMany(e => e.Calificaciones)
                .HasForeignKey(c => c.EstudianteId);
        }
    }
}
