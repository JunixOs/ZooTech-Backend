using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Reports;
using ZooTech.Infrastructure.Repositories;

namespace ZooTech.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ============================================
        // Connection String
        // ============================================

        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        // ============================================
        // DbContext
        // ============================================

        services.AddDbContext<GanaderiaDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        // ============================================
        // Repositories
        // ============================================

        // services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IAnimalReportRepository, AnimalReportRepository>();
        services.AddScoped<IAnimalReportExcelService, AnimalReportExcelService>();

        // ============================================
        // External Services
        // ============================================

        // services.AddScoped<IJwtService, JwtService>();
        // services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        // ============================================
        // Caching
        // ============================================

        // services.AddMemoryCache();

        return services;
    }
}
