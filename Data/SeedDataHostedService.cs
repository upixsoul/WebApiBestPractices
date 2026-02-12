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

                        // Simulación de error solo en el primer intento (para pruebas)
                        if (retryCount == 0)
                        {
                            throw new Exception("Simulación de error en el seed para probar la resiliencia. El seed se reintentará automáticamente.");
                        }

                        // Lista de nombres y apellidos comunes
                        var nombres = new[] { "Juan", "María", "Carlos", "Ana", "Luis", "Sofía", "Miguel", "Laura", "José", "Elena", "Daniel", "Valeria", "Alejandro", "Camila", "Diego", "Isabella", "Fernando", "Gabriela", "Rafael", "Lucía", "Pablo", "Victoria", "Andrés", "Mariana" };
                        var apellidos = new[] { "Pérez", "González", "Hernández", "Martínez", "López", "García", "Rodríguez", "Sánchez", "Ramírez", "Torres", "Flores", "Vázquez", "Ramos", "Morales", "Ortiz", "Jiménez", "Cruz", "Reyes", "Díaz", "Mendoza", "Castro", "Ruiz", "Gutiérrez", "Mendoza" };

                        var estudiantes = new List<Estudiante>();
                        var calificaciones = new List<Calificacion>();

                        var materias = new[] { "Matemáticas", "Español", "Ciencias", "Historia", "Inglés", "Biología", "Química", "Física", "Educación Física", "Arte", "Música", "Geografía" };

                        for (int i = 0; i < 24; i++)
                        {
                            var nombre = nombres[i % nombres.Length];
                            var apellido = apellidos[i % apellidos.Length] + (i % 3 == 0 ? " " + apellidos[(i + 7) % apellidos.Length] : "");

                            var estudiante = new Estudiante
                            {
                                Nombre = nombre,
                                Apellido = apellido,
                                FechaNacimiento = new DateTime(2003 + (i % 6), 1 + (i % 12), 1 + (i % 28))
                            };

                            estudiantes.Add(estudiante);
                            db.Estudiantes.Add(estudiante);
                        }

                        await db.SaveChangesAsync(stoppingToken);

                        // Asignar calificaciones (variedad: algunos con muchas, otros con pocas)
                        var random = new Random(42); // semilla fija para reproducibilidad

                        foreach (var est in estudiantes)
                        {
                            int numCalif = random.Next(0, 7); // 0 a 6 calificaciones por estudiante

                            for (int j = 0; j < numCalif; j++)
                            {
                                var materia = materias[random.Next(materias.Length)];
                                var nota = Math.Round((decimal)(random.NextDouble() * 5 + 5), 1); // entre 5.0 y 10.0
                                var fecha = new DateTime(2025, random.Next(1, 13), random.Next(1, 29));

                                calificaciones.Add(new Calificacion
                                {
                                    EstudianteId = est.Id,
                                    Materia = materia,
                                    Nota = nota,
                                    Fecha = fecha
                                });
                            }
                        }

                        db.Calificaciones.AddRange(calificaciones);
                        await db.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Seed completado: {EstudiantesCount} estudiantes y {CalificacionesCount} calificaciones agregadas.",
                            await db.Estudiantes.CountAsync(stoppingToken),
                            await db.Calificaciones.CountAsync(stoppingToken));

                        return; // Éxito
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
                        return;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }
    }
}
