using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Controllers
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(VacunoController).Assembly);

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
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();