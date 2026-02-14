using Scalar.AspNetCore;

namespace StudentsApi.Extensions
{
    public static class OpenApiExtensions
    {
        public static WebApplication UseCustomScalar(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("Students API - .NET 9");
                    options.WithTheme(ScalarTheme.BluePlanet);
                    options.ExpandAllTags();
                });
            }
            return app;
        }
    }
}
