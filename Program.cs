using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // para keyed services
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using StudentsApi.Data;
using StudentsApi.Dtos;
using StudentsApi.Exceptions; // our custom exception handlers
using StudentsApi.Extensions;
using StudentsApi.Models;
using StudentsApi.Options;
using StudentsApi.Services;
using StudentsApi.Validators;


var builder = WebApplication.CreateBuilder(args);


// ==================== Options Pattern ====================
builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("App"));
var appOptions = builder.Configuration.GetSection("App").Get<AppOptions>() ?? new AppOptions();

// Set environment from options
if (!string.IsNullOrEmpty(appOptions.Environment))
{
    builder.Environment.EnvironmentName = appOptions.Environment;
}

// ==================== Logging (moved to extension) ====================
builder.AddCustomLogging(appOptions);

// ==================== Services Registration ====================
builder.Services.AddOpenApi(); // Native OpenAPI support in .NET 9
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("StudentsDb"));

// Mapster (moved to extension)
builder.Services.AddCustomMappings();

// Register all FluentValidation validators from the assembly
// This single line is enough to register every validator in the same assembly as CreateStudentValidator
builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Application services
builder.Services.AddScoped<IStudentService, StudentService>();

// Custom exception handlers (registered in order of priority)
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ==================== Keyed Services + Option Pattern para seeding ====================
// Registramos ambos como keyed services
builder.Services.AddKeyedSingleton<IHostedService, SeedDataHostedService>("default-seed");
builder.Services.AddKeyedSingleton<IHostedService, OptionalSeedDataHostedService>("optional-seed");

// Wrapper que elige según la opción (solo se registra UNO como IHostedService real)
builder.Services.AddSingleton<IHostedService>(sp =>
{
    var options = sp.GetRequiredService<IOptions<AppOptions>>().Value;
    var key = options.SeedingStrategy.ToLower() == "optional" ? "optional-seed" : "default-seed";
    return sp.GetRequiredKeyedService<IHostedService>(key);
});

var app = builder.Build();

// ==================== Middleware ====================
app.UseCustomScalar(); // Scalar moved to extension

app.UseSerilogRequestLogging(); // Automatic request/response logging

app.UseExceptionHandler(); // Activates our IExceptionHandler implementations

app.UseAuthorization();

app.MapControllers();

app.Run();