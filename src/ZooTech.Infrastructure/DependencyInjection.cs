using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.Ports;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Infrastructure.Caching;
using ZooTech.Infrastructure.Common.Services.ExcelGenerator;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_ProduccionLeche.Repositories;
using ZooTech.Infrastructure.Common.Services.PdfGenerator;

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

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontró ConnectionStrings:DefaultConnection.");

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

        services.AddScoped<IOrdenioRepository, OrdenioRepository>();

        // ============================================
        // External Services
        // ============================================

        services.AddScoped<IExcelGeneratorService, ExcelGeneratorService>();
        services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();

        // ============================================
        // Caching
        // ============================================

        services.AddSingleton<GarnetCacheConnection>();
        services.AddSingleton<IAppCacheService, GarnetCacheService>();

        return services;
    }
}
