using Microsoft.Extensions.DependencyInjection;



using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;

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
        // services.AddScoped<ListarVacunosPresenter>();
        // services.AddScoped<IListarVacunosOutputPort>(sp => sp.GetRequiredService<ListarVacunosPresenter>());

        // services.AddScoped<GenerarArbolGenealogicoPresenter>();
        // services.AddScoped<IGenerarArbolGenealogicoOutputPort>(sp => sp.GetRequiredService<GenerarArbolGenealogicoPresenter>());
        services.AddScoped<IVacunoReferenceResolver, VacunoReferenceResolver>();

        return services;
    }
}
