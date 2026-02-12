using StudentsApi.Data;
using StudentsApi.Dtos;
using StudentsApi.Exceptions; // our custom exception handlers
using StudentsApi.Models;
using StudentsApi.Services;
using StudentsApi.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ==================== Serilog Configuration ====================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

builder.Host.UseSerilog();

// ==================== Services Registration ====================
builder.Services.AddOpenApi(); // Native OpenAPI support in .NET 9

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("StudentsDb"));

// Seed data on startup
builder.Services.AddHostedService<SeedDataHostedService>();

// Mapster DI registration
builder.Services.AddMapster();

// Custom Mapster mappings (after AddMapster())
TypeAdapterConfig<CreateStudentDto, Student>
    .NewConfig()
    .Map(dest => dest.DateOfBirth, src => src.DateOfBirth);

TypeAdapterConfig<Student, StudentResponseDto>
    .NewConfig()
    .Map(dest => dest.Id, src => src.Id);

// Register all FluentValidation validators from the assembly
// This single line is enough to register every validator in the same assembly as CreateStudentValidator
builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentValidator>();

builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddFluentValidationClientsideAdapters(); // opcional, para validación en frontend si usas JS

// Application services
builder.Services.AddScoped<IStudentService, StudentService>();

// Custom exception handlers (registered in order of priority)
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// ==================== Middleware Pipeline ====================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Students API - .NET 9");
        options.WithTheme(ScalarTheme.Mars); // modern and clean look
        options.ExpandAllTags();
    });
}

app.UseSerilogRequestLogging(); // Automatic request/response logging

app.UseExceptionHandler();      // Activates our IExceptionHandler implementations

app.UseAuthorization();

app.MapControllers();

app.Run();