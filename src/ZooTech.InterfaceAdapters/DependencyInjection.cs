using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;

namespace ZooTech.InterfaceAdapters;

public static class DependencyInjection
{
    public static IServiceCollection AddInterfaceAdapters(
        this IServiceCollection services)
    {
        // services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddScoped<TenantHeaderFilter>();
        services.AddScoped<AnonymousOnlyFilter>();

        // Desactivar los mensajes automaticos de validacion de
        // ASP.NET Core
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        // ============================================
        // Filters
        // ============================================

        // services.AddScoped<ValidationFilter>();
        services.AddScoped<IVacunoReferenceResolver, VacunoReferenceResolver>();

        return services;
    }
}
