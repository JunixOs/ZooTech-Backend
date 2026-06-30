using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacunos.UseCases.ListarVacunos;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;
using ZooTech.InterfaceAdapters.Modules.Module_Vacunos.Presenters;

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
        services.AddScoped<ListarVacunosPresenter>();
        services.AddScoped<IListarVacunosOutputPort>(sp => sp.GetRequiredService<ListarVacunosPresenter>());

        services.AddScoped<GenerarArbolGenealogicoPresenter>();
        services.AddScoped<IGenerarArbolGenealogicoOutputPort>(sp => sp.GetRequiredService<GenerarArbolGenealogicoPresenter>());
        services.AddScoped<IVacunoReferenceResolver, VacunoReferenceResolver>();
        services.AddScoped<IVacunoResponseEnricher, VacunoResponseEnricher>();
        services.AddScoped<IVacunoMutationService, VacunoMutationService>();

        return services;
    }
}
