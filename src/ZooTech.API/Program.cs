using ZooTech.Application;
using ZooTech.Infrastructure;

using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;

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