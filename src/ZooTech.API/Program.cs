using System.Reflection;
using Microsoft.Extensions.FileProviders;
using ZooTech.Application;
using ZooTech.Infrastructure;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Middleware;

// ─────────────────────────────────────────────────────────────────────────────
// GET /vacunos/reportes/listado
// ─────────────────────────────────────────────────────────────────────────────

var builder = WebApplication.CreateBuilder(args);

// ── Capas de la arquitectura ─────────────────────────────────────────────────
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddInterfaceAdapters();

// ── Controladores ─────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddApplicationPart(
        typeof(ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno
               .Controllers.ReportesVacunosController).Assembly);

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v2", new()
    {
        Title = "Módulo Vacuno",
        Version = "v2",
        Description = "Endpoint GET /vacunos/reportes/listado – " +
                      "Reporte listado de vacunos con filtros, búsqueda y paginación."
    });

    // Incluir comentarios XML del proyecto API
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    // Incluir comentarios XML de InterfaceAdapters
    var adaptersXml = Path.Combine(
        AppContext.BaseDirectory,
        "ZooTech.InterfaceAdapters.xml");
    if (File.Exists(adaptersXml))
        options.IncludeXmlComments(adaptersXml);
});

// ── Logging ───────────────────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>()
            ?? Array.Empty<string>();

        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            var frontendUrl = builder.Configuration.GetValue<string>("Frontend:FrontendUrl");
            if (!string.IsNullOrEmpty(frontendUrl))
            {
                policy.WithOrigins(frontendUrl)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
        }
    });
});

var app = builder.Build();

// ─────────────────────────────────────────────────────────────────────────────
// Pipeline HTTP
// ─────────────────────────────────────────────────────────────────────────────

app.UseMiddleware<ExceptionHandlingMiddleware>();

var swaggerEnabled = app.Environment.IsDevelopment()
    || app.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "ZooTech TK02 v2");
        c.RoutePrefix = string.Empty; // Swagger en raíz: http://localhost:5085
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

var generatedFilesRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
Directory.CreateDirectory(generatedFilesRoot);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(generatedFilesRoot),
    RequestPath = string.Empty
});

app.UseCors("FrontendCors");
app.MapControllers();

app.Run();
