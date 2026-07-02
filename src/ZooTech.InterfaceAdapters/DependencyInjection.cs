using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Presenters;

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

        services.AddScoped<GenerarArbolGenealogicoPresenter>();
        services.AddScoped<IGenerarArbolGenealogicoOutputPort>(sp => sp.GetRequiredService<GenerarArbolGenealogicoPresenter>());

        return services;
    }
}