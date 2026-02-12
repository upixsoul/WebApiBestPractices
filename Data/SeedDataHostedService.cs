using EstudiantesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EstudiantesApi.Data
{
    public class SeedDataHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SeedDataHostedService> _logger;

        public SeedDataHostedService(
            IServiceProvider serviceProvider,
            ILogger<SeedDataHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // BackgroundService usa ExecuteAsync en lugar de StartAsync
            // y permite que la app continúe aunque este método falle

            const int maxRetries = 5;
            int retryCount = 0;

            while (retryCount < maxRetries && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (db.Database.IsInMemory() || !await db.Estudiantes.AnyAsync(stoppingToken))
                    {
                        _logger.LogInformation("Iniciando seed de datos iniciales (intento {RetryCount}/{MaxRetries})...", retryCount + 1, maxRetries);

                        if (retryCount.Equals(0))
                        {
                            throw new Exception("Simulación de error en el seed para probar la resiliencia. El seed se reintentará automáticamente.");
                        }

                        // Estudiante 1
                        var e1 = new Estudiante
                        {
                            Nombre = "Juan",
                            Apellido = "Pérez López",
                            FechaNacimiento = new DateTime(2005, 5, 15)
                        };
                        db.Estudiantes.Add(e1);

                        // Estudiante 2
                        var e2 = new Estudiante
                        {
                            Nombre = "María",
                            Apellido = "González Ramírez",
                            FechaNacimiento = new DateTime(2006, 8, 22)
                        };
                        db.Estudiantes.Add(e2);

                        // Estudiante 3
                        var e3 = new Estudiante
                        {
                            Nombre = "Carlos",
                            Apellido = "Hernández Soto",
                            FechaNacimiento = new DateTime(2004, 11, 3)
                        };
                        db.Estudiantes.Add(e3);

                        // Estudiante 4 (con pocas calificaciones)
                        var e4 = new Estudiante
                        {
                            Nombre = "Ana",
                            Apellido = "Martínez Vargas",
                            FechaNacimiento = new DateTime(2007, 2, 14)
                        };
                        db.Estudiantes.Add(e4);

                        // Guardamos los estudiantes para obtener sus IDs
                        await db.SaveChangesAsync(stoppingToken);

                        // Calificaciones para Estudiante 1 (Juan)
                        db.Calificaciones.AddRange(
                            new Calificacion { EstudianteId = e1.Id, Materia = "Matemáticas", Nota = 9.8m, Fecha = new DateTime(2025, 6, 10) },
                            new Calificacion { EstudianteId = e1.Id, Materia = "Español", Nota = 8.5m, Fecha = new DateTime(2025, 5, 20) },
                            new Calificacion { EstudianteId = e1.Id, Materia = "Ciencias", Nota = 9.2m, Fecha = new DateTime(2025, 7, 5) },
                            new Calificacion { EstudianteId = e1.Id, Materia = "Historia", Nota = 7.9m, Fecha = new DateTime(2025, 4, 15) }
                        );

                        // Calificaciones para Estudiante 2 (María)
                        db.Calificaciones.AddRange(
                            new Calificacion { EstudianteId = e2.Id, Materia = "Matemáticas", Nota = 6.5m, Fecha = new DateTime(2025, 6, 12) },
                            new Calificacion { EstudianteId = e2.Id, Materia = "Inglés", Nota = 9.0m, Fecha = new DateTime(2025, 5, 25) },
                            new Calificacion { EstudianteId = e2.Id, Materia = "Biología", Nota = 8.7m, Fecha = new DateTime(2025, 7, 8) },
                            new Calificacion { EstudianteId = e2.Id, Materia = "Educación Física", Nota = 10.0m, Fecha = new DateTime(2025, 6, 1) }
                        );

                        // Calificaciones para Estudiante 3 (Carlos)
                        db.Calificaciones.AddRange(
                            new Calificacion { EstudianteId = e3.Id, Materia = "Matemáticas", Nota = 5.5m, Fecha = new DateTime(2025, 6, 8) },
                            new Calificacion { EstudianteId = e3.Id, Materia = "Química", Nota = 4.8m, Fecha = new DateTime(2025, 5, 18) },
                            new Calificacion { EstudianteId = e3.Id, Materia = "Física", Nota = 6.2m, Fecha = new DateTime(2025, 7, 3) }
                        );

                        // Calificaciones para Estudiante 4 (Ana) – pocas para variar
                        db.Calificaciones.AddRange(
                            new Calificacion { EstudianteId = e4.Id, Materia = "Arte", Nota = 9.5m, Fecha = new DateTime(2025, 6, 5) },
                            new Calificacion { EstudianteId = e4.Id, Materia = "Música", Nota = 8.0m, Fecha = new DateTime(2025, 5, 30) }
                        );

                        await db.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Seed completado: {EstudiantesCount} estudiantes y {CalificacionesCount} calificaciones agregadas.",
                            await db.Estudiantes.CountAsync(stoppingToken),
                            await db.Calificaciones.CountAsync(stoppingToken));
                        return; // ¡Éxito! Salimos del loop
                    }
                    else
                    {
                        _logger.LogInformation("La base de datos ya contiene datos. Seed omitido.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning(ex, "Error al ejecutar seed (intento {RetryCount}/{MaxRetries}). Reintentando en 5 segundos...", retryCount, maxRetries);

                    if (retryCount >= maxRetries)
                    {
                        _logger.LogError(ex, "Falló el seed después de {MaxRetries} intentos. La aplicación continuará sin seed.");
                        return; // ← ¡Importante! No lanzamos excepción → la app sigue viva
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
