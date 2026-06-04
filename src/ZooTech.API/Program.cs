using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ZooTech.API.Configuration;
using ZooTech.API.Endpoints;
using ZooTech.API.Services;
using ZooTech.Application;
using ZooTech.Infrastructure;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInterfaceAdapters();
builder.Services.Configure<VacunoRequirementOptions>(
    builder.Configuration.GetSection(VacunoRequirementOptions.SectionName));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddApplicationPart(typeof(ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Controllers.ReportesVacunosController).Assembly);
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ZooTech API",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese: Bearer zootech-demo-token",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
});

builder.Services.AddSingleton<IDemoAuthService, DemoAuthService>();
// If a connection string is provided use EF repository against SQL Server, otherwise use in-memory
var defaultConn = builder.Configuration.GetConnectionString("Default");
if (!builder.Environment.IsEnvironment("Testing") && !string.IsNullOrWhiteSpace(defaultConn))
{
    builder.Services.AddDbContext<ZooTech.Infrastructure.Persistence.Context.GanaderiaDbContext>(options =>
        options.UseSqlServer(defaultConn));

    builder.Services.AddScoped<IVacunoRepository, ZooTech.API.Services.EfVacunoRepository>();
}
else
{
    builder.Services.AddSingleton<IVacunoRepository, InMemoryVacunoRepository>();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                 uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
                 uri.Host.Equals("::1", StringComparison.OrdinalIgnoreCase)))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

var generatedFilesRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
Directory.CreateDirectory(generatedFilesRoot);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(generatedFilesRoot),
    RequestPath = string.Empty
});

app.MapGet("/api/health", () => Results.Ok(new
    {
        status = "ok",
        service = "ZooTech.API",
        utc = DateTime.UtcNow
    }))
    .WithTags("Sistema")
    .WithName("HealthCheck")
    .WithSummary("Verifica que la API este disponible.");

app.MapAuthEndpoints();
app.MapVacunoEndpoints();
app.MapControllers();

app.Run();

public partial class Program;


