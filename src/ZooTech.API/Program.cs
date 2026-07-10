using Microsoft.EntityFrameworkCore;
using ZooTech.Application;
using ZooTech.Infrastructure;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Controllers;
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Controllers;
using QuestPDF.Infrastructure;

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

// ======= Configuracion Capas =======
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInterfaceAdapters();

// ======= Configuracion Context BD Tenant Principal =======
builder.Services.AddDbContext<TenantCatalogDb>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("TenantCatalogConnection"));
});

// ======= Configuracion DI =======
builder.Services.AddMemoryCache();

// ======= Configuracion CORS =======
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    return false;

                // Produccion: permite zentrycorp.dev y todos sus subdominios
                var isZentryDomain =
                    uri.Scheme == "https" &&
                    (
                        uri.Host == "zentrycorp.dev" ||
                        uri.Host.EndsWith(".zentrycorp.dev")
                    );

                // Desarrollo local

                var isLocal =
                    uri.Scheme == "http" &&
                    uri.Port == 4200 &&
                    (
                        uri.Host == "admin.zentrycorp.local" ||
                        uri.Host == "zootecniaunas.zentrycorp.local" ||
                        uri.Host == "elroble.zentrycorp.local" ||
                        uri.Host == "lacteosdelvalle.zentrycorp.local" ||
                        uri.Host == "losandes.zentrycorp.local" ||
                        uri.Host == "localhost" ||
                        uri.Host == "127.0.0.1"
                    );
                return isZentryDomain || isLocal;
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
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

// ======= CORS =======
// IMPORTANTE: debe ir antes de TenantResolution, Authentication y Authorization.
app.UseCors("AllowFrontend");


// ======= JWT =======
app.UseAuthentication();
app.UseAuthorization();

// ======= Swagger =======
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
}
// ======= Tenant Middleware =======
app.UseMiddleware<TenantResolutionMiddleware>();

// ======= Controllers =======
app.MapControllers();

app.Run();

public partial class Program { }