using Microsoft.Extensions.DependencyInjection;

namespace ZooTech.InterfaceAdapters;

public static class DependencyInjection
{
    public static IServiceCollection AddInterfaceAdapters(
        this IServiceCollection services)
    {
        // services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        return services;
    }
}
