using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.InterfaceAdapters.Filters;

namespace ZooTech.InterfaceAdapters;

public static class DependencyInjection
{
    public static IServiceCollection AddInterfaceAdapters(
        this IServiceCollection services)
    {
        // services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddScoped<TenantHeaderFilter>();

        // Desactivar los mensajes automaticos de validacion de
        // ASP.NET Core
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }
}
