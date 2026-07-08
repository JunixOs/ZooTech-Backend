using Microsoft.Extensions.DependencyInjection;

namespace ZooTech.InterfaceAdapters;

public static class DependencyInjection
{
    public static IServiceCollection AddInterfaceAdapters(
        this IServiceCollection services)
    {
        // ============================================
        // Presenters
        // ============================================

        // services.AddScoped<IAnimalPresenter, AnimalPresenter>();

        // ============================================
        // Mappers
        // ============================================

        // services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // ============================================
        // Filters
        // ============================================

        // services.AddScoped<ValidationFilter>();
        services.AddScoped<ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services.IVacunoReferenceResolver, ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services.VacunoReferenceResolver>();

        return services;
    }
}