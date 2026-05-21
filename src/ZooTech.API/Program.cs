using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Presenters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

// ======= Configuracion Swagger =======
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(HomeController).Assembly);
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
});

builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);

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

builder.Services.AddScoped<ITenantStore , TenantStore>();
builder.Services.AddScoped<ITenantContext , TenantContext>();
builder.Services.AddScoped<ITenantProvisioningService , TenantProvisioningService>();

builder.Services.AddTransient<ICreateTenantInputPort , CreateTenantInteractor>();
builder.Services.AddTransient<TenancingPresenter>();
builder.Services.AddScoped<
    ICreateTenantOutputPort>(
        sp => sp.GetRequiredService<
            TenancingPresenter>());
// ======= Configuracion DI =======

var frontendPort = builder.Configuration["Frontend:FrontendPort"];
var frontendIP = builder.Configuration["Frontend:FrontendIP"];
var frontendProtocol = builder.Configuration["Frontend:FrontendProtocol"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
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
    });

    // app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();