using ZooTech.Infrastructure.Configuration;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Infrastructure.Common.Time;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSanidadServices(builder.Configuration);
//Time
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();