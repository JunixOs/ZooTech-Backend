using ZooTech.Application;
using ZooTech.Infrastructure;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Controllers;
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Controllers;
using QuestPDF.Infrastructure;
using ZooTech.Application.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ======= Configuracion Controllers =======
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(HomeController).Assembly)
    .AddApplicationPart(typeof(CeloController).Assembly)
    .AddApplicationPart(typeof(VacunoController).Assembly)
    .AddApplicationPart(typeof(ProduccionLecheController).Assembly)
    .AddApplicationPart(typeof(TenancingController).Assembly);

// ======= Configuracion Swagger =======
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("administracion", new()
    {
        // El nombre de aqui define el titulo para la página o doc Swagger
        Title = "Administración Endpoints",
        Version = "v1"
    });
    options.SwaggerDoc("gestionusuarios", new()
    {
        Title = "Gestión de Usuarios Endpoints",
        Version = "v1"
    });

    // Los nombres aqui deben coincidir con los de options.SwaggerEndpoint()
    options.SwaggerDoc("reproduccion-celo", new()
    {
        Title = "Reproducción - Celo Endpoints",
        Version = "v1"
    });
    options.SwaggerDoc("reproduccion-fecundacion", new()
    {
        Title = "Reproducción - Fecundación Endpoints",
        Version = "v1"
    });
    options.SwaggerDoc("sanidad-triaje", new()
    {
        Title = "Sanidad - Triaje Endpoints",
        Version = "v1"
    });
    options.SwaggerDoc("vacuno", new()
    {
        Title = "Vacuno Endpoints",
        Version = "v1"
    });
    options.SwaggerDoc("produccionleche", new()
    {
        Title = "Producción de Leche Endpoints",
        Version = "v1"
    });
});

// ======= Configuracion Capas =======
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInterfaceAdapters();

// ======= Configuracion DI =======
builder.Services.AddMemoryCache();

// ======= Configuracion CORS =======
builder.Services.AddCors(options =>
{
    string base_domain = builder.Configuration["MultiTenant:BaseDomain"] ?? throw new NullReferenceException(
                    message: "Missing configuration: MultiTenant:BaseDomain"
                );

    options.AddPolicy("ZooTechCorsPolicy", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    return false;

                // Produccion: permite el dominio definido en variables de entorno y todos sus subdominios
                var isZentryDomain =
                    (uri.Scheme == "http" || uri.Scheme == "https") &&
    		    (
        		uri.Host == base_domain ||
        		uri.Host.EndsWith($".{base_domain}")
      		    );

                // Desarrollo local
                var isLocalhost =
                    (uri.Scheme == "http" || uri.Scheme == "https") &&
                    (
                        uri.Host == "localhost" ||
                        uri.Host == "127.0.0.1"
                    );

                return isZentryDomain || isLocalhost;
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders(
                "X-Tenant-Id",
                "X-Tenant-Name",
                "X-Tenant-Legal-Name",
                "X-Tenant-Type");
    });
});

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// ======= Middleware global de errores =======
app.UseMiddleware<ExceptionHandlingMiddleware>();

// ======= HTTPS =======
app.UseHttpsRedirection();

// ======= Routing =======
app.UseRouting();


// ======= Swagger =======
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/administracion/swagger.json",
            // El nombre de aqui define lo que se mostrará en el selector de Swagger
            "Administración");
        options.SwaggerEndpoint(
            "/swagger/gestionusuarios/swagger.json",
            "Gestión de Usuarios");

        // El nombre entre /swagger/**/swagger.json debe ser igual al definido en options.SwaggerDoc()
        options.SwaggerEndpoint(
            "/swagger/reproduccion-celo/swagger.json",
            "Reproducción - Celo");
        options.SwaggerEndpoint(
            "/swagger/reproduccion-fecundacion/swagger.json",
            "Reproducción - Fecundación");
        options.SwaggerEndpoint(
            "/swagger/sanidad-triaje/swagger.json",
            "Sanidad - Triaje");
        options.SwaggerEndpoint(
            "/swagger/vacuno/swagger.json",
            "Vacuno");
        options.SwaggerEndpoint(
            "/swagger/produccionleche/swagger.json",
            "Producción de Leche");
    });

}
// ======= CORS =======
// IMPORTANTE: debe ir antes de TenantResolution, Authentication y Authorization.

app.UseCors("ZooTechCorsPolicy");

// ======= Tenant Middleware =======
app.UseMiddleware<TenantResolutionMiddleware>();

// ======= JWT =======
app.UseAuthentication();
app.UseAuthorization();

// ======= Controllers =======
app.MapControllers();

app.Run();

public partial class Program { }