using ZooTech.Application;
using ZooTech.Infrastructure;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Controllers;
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(HomeController).Assembly)
    .AddApplicationPart(typeof(CeloController).Assembly)
    .AddApplicationPart(typeof(VacunoController).Assembly)
    .AddApplicationPart(typeof(ProduccionLecheController).Assembly);

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

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddInterfaceAdapters();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var frontendPort = builder.Configuration["Frontend:FrontendPort"];
        var frontendIP = builder.Configuration["Frontend:FrontendIP"];
        var frontendProtocol = builder.Configuration["Frontend:FrontendProtocol"];

        policy.WithOrigins($"{frontendProtocol}://{frontendIP}:{frontendPort}")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

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


app.UseMiddleware<ExceptionHandlingMiddleware>();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
