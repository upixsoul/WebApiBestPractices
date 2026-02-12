using EstudiantesApi.Data;
using EstudiantesApi.Dtos;
using EstudiantesApi.Exceptions;           // nuestros handlers
using EstudiantesApi.Models;
using EstudiantesApi.Services;
using EstudiantesApi.Validators;
using FluentValidation;
using Mapster;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==================== Serilog ====================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

builder.Host.UseSerilog();

// ==================== Servicios ====================
builder.Services.AddOpenApi();                                   // OpenAPI nativo .NET 9
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EstudiantesDb"));

builder.Services.AddHostedService<SeedDataHostedService>();

builder.Services.AddMapster();                                   // Mapster DI

// Agrega esto después de AddMapster()
TypeAdapterConfig<CreateEstudianteDto, Estudiante>
    .NewConfig()
    .Map(dest => dest.FechaNacimiento, src => src.FechaNacimiento);

TypeAdapterConfig<Estudiante, EstudianteResponseDto>
    .NewConfig()
    .Map(dest => dest.Id, src => src.Id);

// Validadores FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateEstudianteValidator>();

// Services
builder.Services.AddScoped<IEstudianteService, EstudianteService>();

// Handlers de excepciones (orden = prioridad)
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// ==================== Pipeline ====================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Estudiantes API - .NET 9");
        options.WithTheme(ScalarTheme.Mars);          // bonito y moderno
        options.ExpandAllTags();
    });
}

app.UseSerilogRequestLogging();          // log automático de cada request
app.UseExceptionHandler();               // usa los IExceptionHandler

app.UseAuthorization();
app.MapControllers();

app.Run();