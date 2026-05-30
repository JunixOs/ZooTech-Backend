using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.RegistrarVacuno;
using ZooTech.Infrastructure.Features;
using ZooTech.Infrastructure.Persistence;
using ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno;
using ZooTech.Infrastructure.Time;
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

var builder = WebApplication.CreateBuilder(args);

// DbContext SQL Server
builder.Services.AddDbContext<GanaderiaDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? builder.Configuration.GetConnectionString("DefaultString")
        ?? throw new InvalidOperationException("Falta la cadena de conexion DefaultConnection o DefaultString.");

    options.UseSqlServer(connectionString);
});

// Controllers
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(VacunoController).Assembly);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var details = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors.Select(error => new
            {
                field = x.Key,
                message = string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "Campo invalido."
                    : error.ErrorMessage
            }));

        return new BadRequestObjectResult(new
        {
            error = new
            {
                code = "VALIDATION_ERROR",
                message = "Los datos enviados no son validos.",
                details
            }
        });
    };
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("public", new() { Title = "Public API", Version = "v1" });
    options.SwaggerDoc("auth", new() { Title = "Authentication API", Version = "v1" });
    options.SwaggerDoc("users", new() { Title = "Users API", Version = "v1" });
});

// MediatR
builder.Services.AddMediatR(typeof(RegistrarVacunoHandler).Assembly);

// Pipeline de validación
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(RegistrarVacunoValidator).Assembly);

// Repositorios y servicios
builder.Services.AddScoped<IVacunoRepository, VacunoRepository>();
builder.Services.AddScoped<IArchivoService, ArchivoService>();
builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();

// Configuración para multipart/form-data
builder.Services.Configure<FormOptions>(opt =>
{
    opt.MultipartBodyLengthLimit = 5 * 1024 * 1024; // 5 MB
});

// CORS
var frontendPort = builder.Configuration["Frontend:FrontendPort"];
var frontendIP = builder.Configuration["Frontend:FrontendIP"];
var frontendProtocol = builder.Configuration["Frontend:FrontendProtocol"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins($"{frontendProtocol}://{frontendIP}:{frontendPort}")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/public/swagger.json", "Public API");
        options.SwaggerEndpoint("/swagger/auth/swagger.json", "Authentication API");
        options.SwaggerEndpoint("/swagger/users/swagger.json", "Users API");
    });
}

// Middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
var storageBasePath = builder.Configuration["Storage:BasePath"];
if (!string.IsNullOrWhiteSpace(storageBasePath))
{
    Directory.CreateDirectory(storageBasePath);
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(storageBasePath),
        RequestPath = "/files"
    });
}
app.UseCors("AllowFrontend");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;
