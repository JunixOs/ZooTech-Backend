using ZooTech.Application;
using ZooTech.Infrastructure;
using ZooTech.InterfaceAdapters;

using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacunos.Controllers;

var builder = WebApplication.CreateBuilder(args);

// ======= Controllers & Swagger =======
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(HomeController).Assembly)
    .AddApplicationPart(typeof(VacunosController).Assembly);
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

// ======= Capas de la Arquitectura =======
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInterfaceAdapters();

// ======= CORS =======
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

// ======= HTTP Pipeline =======
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

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Middleware: Resolución de Tenant desde header X-Tenant-Id
// El TenantContext ya lee el header internamente vía IHttpContextAccessor,
// pero este log ayuda a diagnosticar qué tenant se está usando.
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("TenantMiddleware");

    if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader))
    {
        logger.LogInformation("Tenant recibido por header: {TenantId}", tenantHeader.ToString());
    }
    else
    {
        logger.LogInformation("Sin header X-Tenant-Id, usando DefaultTenantId del appsettings");
    }

    await next();
});

app.UseAuthorization();
app.MapControllers();

app.Run();
