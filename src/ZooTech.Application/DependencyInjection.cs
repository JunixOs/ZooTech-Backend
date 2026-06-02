using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Modules.Module_Vacunos.UseCases.GenerarArbolGenealogico;
using ZooTech.Application.Modules.Module_Vacunos.UseCases.ListarVacunos;

namespace ZooTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // ============================================
        // MediatR
        // ============================================

        // services.AddMediatR(cfg =>
        // {
        //     cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        // });

        // ============================================
        // FluentValidation
        // ============================================

        // services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // ============================================
        // Pipeline Behaviors
        // ============================================

        // services.AddTransient(
        //     typeof(IPipelineBehavior<,>),
        //     typeof(ValidationBehavior<,>));

        // ============================================
        // Use Cases / Services
        // ============================================

        // services.AddScoped<IMyService, MyService>();
        services.AddScoped<IListarVacunosInputPort, ListarVacunosInteractor>();
        services.AddScoped<IGenerarArbolGenealogicoInputPort, GenerarArbolGenealogicoInteractor>();

        return services;
    }
}