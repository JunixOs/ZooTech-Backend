using Microsoft.Extensions.DependencyInjection;
using ZooTech.InterfaceAdapters.Middleware;

namespace ZooTech.InterfaceAdapters;

public static class DependencyInjection
{
    public static IServiceCollection AddInterfaceAdapters(
        this IServiceCollection services)
    {
        // services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddScoped<TenantHeaderFilter>();
        
        return services;
    }
}
