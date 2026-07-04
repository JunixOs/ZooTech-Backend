using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GenerarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunosPaginado;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Presenters;
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
        services.AddScoped<ListarVacunosPaginadoPresenter>();
        services.AddScoped<IListarVacunosPaginadoOutputPort>(sp => sp.GetRequiredService<ListarVacunosPaginadoPresenter>());

        services.AddScoped<GenerarArbolGenealogicoPresenter>();
        services.AddScoped<IGenerarArbolGenealogicoOutputPort>(sp => sp.GetRequiredService<GenerarArbolGenealogicoPresenter>());
        services.AddScoped<IVacunoReferenceResolver, VacunoReferenceResolver>();
        services.AddScoped<IVacunoResponseEnricher, VacunoResponseEnricher>();
        services.AddScoped<IVacunoMutationService, VacunoMutationService>();

        return services;
    }
}
