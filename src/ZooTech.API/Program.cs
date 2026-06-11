using Asp.Versioning;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ZooTech.Application;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant.Ports;
using ZooTech.Infrastructure;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Controllers;
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Presenters;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(HomeController).Assembly)
    .AddApplicationPart(typeof(CeloController).Assembly)
    .AddApplicationPart(typeof(VacunoController).Assembly)
    .AddApplicationPart(typeof(ProduccionLecheController).Assembly);

// Configuracion del versionado de la API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("auth", new()
    {
        Title = "Authentication API",
        Version = "v1"
    });

    options.SwaggerDoc("users", new()
    {
        Title = "Users API",
        Version = "v1"
    });

    options.SwaggerDoc("public", new()
    {
        Title = "Public API",
        Version = "v1"
    });

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ZooTech API",
        Version = "v1",
        Description = "API para la gestión de la granja ZooTech, incluyendo módulos de producción de leche, alimentación, salud animal, entre otros.",

    });

});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddInterfaceAdapters();

// ======= Configuracion Context BD Tenant Principal =======
builder.Services.AddDbContext<TenantCatalogDb>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("TenantCatalogConnection"));
});
// ======= Configuracion Context BD Tenant Principal =======

// ======= Configuracion DI =======
builder.Services.AddMemoryCache();
// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateTenantHandler)
            .Assembly);
});
// FluentValidation
builder.Services
    .AddValidatorsFromAssembly(
        typeof(CreateTenantValidator)
            .Assembly);

// ======= Registro de Validators =======
builder.Services.AddValidatorsFromAssembly(
    typeof(CreateTenantValidator).Assembly
);
// ======= Registro de Validators =======

// ======= Registro de Behaviors =======
builder.Services.AddTransient(
    typeof(ZooTech.Application.Common.Behaviors.IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);
// ======= Registro de Behaviors =======

builder.Services.AddScoped<ITenantStore, TenantStore>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();

builder.Services.AddTransient<ICreateTenantInputPort, CreateTenantInteractor>();
builder.Services.AddTransient<CreateTenantPresenter>();
builder.Services.AddScoped<
    ICreateTenantOutputPort>(
        sp => sp.GetRequiredService<
            CreateTenantPresenter>());

builder.Services.AddScoped<ITenantDatabaseMigrator, TenantDatabaseMigrator>();
builder.Services.AddScoped<IGanaderiaDbContextFactory, GanaderiaDbContextFactory>();
// ======= Configuracion DI =======

var frontendPort = builder.Configuration["Frontend:FrontendPort"];
var frontendIP = builder.Configuration["Frontend:FrontendIP"];
var frontendProtocol = builder.Configuration["Frontend:FrontendProtocol"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {

     policy.WithOrigins($"{frontendProtocol}://{frontendIP}:{frontendPort}")
    .AllowAnyHeader()
    .AllowAnyMethod();
    });
});


var app = builder.Build();

// ===== Configurar Middlewares =====
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
// ===== Configurar Middlewares =====

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/public/swagger.json",
            "Public API");

        options.SwaggerEndpoint(
            "/swagger/auth/swagger.json",
            "Authentication API");

        options.SwaggerEndpoint(
            "/swagger/users/swagger.json",
            "Users API");

        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "ZooTech API"
        );
    });
}




app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();