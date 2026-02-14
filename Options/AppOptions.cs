using Serilog.Events;

namespace StudentsApi.Options
{
    public class AppOptions
    {
        public string Environment { get; set; } = "Development";
        public LogEventLevel MicrosoftLogLevel { get; set; } = LogEventLevel.Information;
        public string SeedingStrategy { get; set; } = "default"; // "default" or "optional"
    }
}
