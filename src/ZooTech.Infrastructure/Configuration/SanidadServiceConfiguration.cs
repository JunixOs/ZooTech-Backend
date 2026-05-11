using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Domain.Module_Sanidad.Interfaces;
using ZooTech.Infrastructure.Context;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Repositories;

namespace ZooTech.Infrastructure.Configuration;

public static class SanidadServiceConfiguration
{
    public static IServiceCollection AddSanidadServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ZootechContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITriajeRepository, TriajeRepository>();

        return services;
    }
}