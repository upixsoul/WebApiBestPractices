using Microsoft.EntityFrameworkCore;
using StudentsApi.Models;

namespace StudentsApi.Data
{
    public class OptionalSeedDataHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OptionalSeedDataHostedService> _logger;

        public OptionalSeedDataHostedService(
            IServiceProvider serviceProvider,
            ILogger<OptionalSeedDataHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int maxRetries = 7;                    // ← cambiado a 7
            int retryCount = 0;

            while (retryCount < maxRetries && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (db.Database.IsInMemory() || !await db.Students.AnyAsync(stoppingToken))
                    {
                        _logger.LogInformation("Starting OPTIONAL data seeding (attempt {RetryCount}/{MaxRetries})...", retryCount + 1, maxRetries);

                        if (retryCount == 0)
                        {
                            throw new Exception("Simulated seeding failure to test resilience. Will retry automatically.");
                        }

                        if (retryCount == 1)
                        {
                            throw new Exception("Simulated seeding failure number 2 to test resilience. Will retry automatically.");
                        }

                        var firstNames = new[]
                        {
                            "Liam", "Noah", "Oliver", "James", "Elijah", "Mateo", "Theodore", "Henry",
                            "Lucas", "William", "Benjamin", "Levi", "Jack", "Ezra", "Asher",
                            "Olivia", "Emma", "Amelia", "Sophia", "Charlotte", "Isabella", "Mia", "Ava", "Evelyn"
                        };

                        var students = new List<Student>();
                        var qualifications = new List<Qualification>();
                        var subjects = new[] { "Maths", "Spanish", "Science", "History", "English", "Biology", "Chemistry", "Physics", "Physical Education", "Art", "Music", "Geography" };

                        for (int i = 0; i < 24; i++)
                        {
                            var firstName = firstNames[i % firstNames.Length];
                            var student = new Student
                            {
                                FirstName = firstName,
                                DateOfBirth = new DateTime(2003 + (i % 6), 1 + (i % 12), 1 + (i % 28))
                            };
                            students.Add(student);
                            db.Students.Add(student);
                        }

                        await db.SaveChangesAsync(stoppingToken);

                        var random = new Random(42);
                        foreach (var student in students)
                        {
                            int numQualifications = random.Next(0, 7);
                            for (int j = 0; j < numQualifications; j++)
                            {
                                var subject = subjects[random.Next(subjects.Length)];
                                var gradeValue = Math.Round((decimal)(random.NextDouble() * 5 + 5), 1);
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

                        _logger.LogInformation("Optional seeding completed: {StudentCount} students and {GradeCount} grades added.",
                            await db.Students.CountAsync(stoppingToken),
                            await db.Qualifications.CountAsync(stoppingToken));

                        return;
                    }
                    else
                    {
                        _logger.LogInformation("Database already contains data. Optional seeding skipped.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning(ex, $"Error during optional seeding (attempt {retryCount}/{maxRetries}). Retrying in 5 seconds...");
                    if (retryCount >= maxRetries)
                    {
                        _logger.LogError(ex, $"Optional seeding failed after {maxRetries} attempts.");
                        return;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }
    }
}
