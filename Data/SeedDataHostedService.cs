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
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            while (retryCount < maxRetries && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Skip seeding if database already has data (except for in-memory DB, which is always empty on startup)
                    if (db.Database.IsInMemory() || !await db.Students.AnyAsync(stoppingToken))
                    {
                        _logger.LogInformation("Starting initial data seeding (attempt {RetryCount}/{MaxRetries})...", retryCount + 1, maxRetries);

                        // Simulate failure on first attempt (for testing retry logic)
                        if (retryCount == 0)
                        {
                            throw new Exception("Simulated seeding failure to test resilience. Will retry automatically.");
                        }

                        // Common first names and last names
                        var firstNames = new[]
                        {
                            "Liam", "Noah", "Oliver", "James", "Elijah", "Mateo", "Theodore", "Henry",
                            "Lucas", "William", "Benjamin", "Levi", "Jack", "Ezra", "Asher",
                            "Olivia", "Emma", "Amelia", "Sophia", "Charlotte", "Isabella", "Mia", "Ava", "Evelyn"
                        };
                        var lastNames = new[]
                        {
                            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
                            "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
                            "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White"
                        };

                        var students = new List<Student>();
                        var qualifications = new List<Qualification>();

                        var subjects = new[] { "Maths", "Spanish", "Science", "History", "English", "Biology", "Chemistry", "Physics", "Physical Education", "Art", "Music", "Geography" };

                        for (int i = 0; i < 24; i++)
                        {
                            var firstName = firstNames[i % firstNames.Length];
                            var lastName = lastNames[i % lastNames.Length];

                            var student = new Student
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                DateOfBirth = new DateTime(2003 + (i % 6), 1 + (i % 12), 1 + (i % 28))
                            };

                            students.Add(student);
                            db.Students.Add(student);
                        }

                        await db.SaveChangesAsync(stoppingToken);

                        // Assign grades (some students with many, some with few)
                        var random = new Random(42); // fixed seed for reproducibility

                        foreach (var student in students)
                        {
                            int numQualifications = random.Next(0, 7); // 0 to 6 grades per student

                            for (int j = 0; j < numQualifications; j++)
                            {
                                var subject = subjects[random.Next(subjects.Length)];
                                var gradeValue = Math.Round((decimal)(random.NextDouble() * 5 + 5), 1); // between 5.0 and 10.0
                                var date = new DateTime(2025, random.Next(1, 13), random.Next(1, 29));

                                qualifications.Add(new Qualification()
                                {
                                    StudentId = student.Id,
                                    Subject = subject,
                                    Grade = gradeValue,
                                    Date = date
                                });
                            }
                        }

                        db.Qualifications.AddRange(qualifications);
                        await db.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Seeding completed: {StudentCount} students and {GradeCount} grades added.",
                            await db.Students.CountAsync(stoppingToken),
                            await db.Qualifications.CountAsync(stoppingToken));

                        return; // Success
                    }
                    else
                    {
                        _logger.LogInformation("Database already contains data. Seeding skipped.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning(ex, $"Error during seeding (attempt {retryCount}/{maxRetries}). Retrying in 5 seconds...", retryCount, maxRetries);

                    if (retryCount >= maxRetries)
                    {
                        _logger.LogError(ex, $"Seeding failed after {maxRetries} attempts. Application will continue without seed data.");
                        return;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }
    }
}
