using Serilog;
using StudentsApi.Options;

namespace StudentsApi.Extensions
{
    public static class LoggingExtensions
    {
        public static WebApplicationBuilder AddCustomLogging(this WebApplicationBuilder builder, AppOptions options)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", options.MicrosoftLogLevel)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            builder.Host.UseSerilog();
            return builder;
        }
    }
}
