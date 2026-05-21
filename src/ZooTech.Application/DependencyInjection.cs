using Microsoft.Extensions.DependencyInjection;

using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

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

        services.AddScoped<IListarReporteVacunosUseCase, ListarReporteVacunosUseCase>();
        services.AddScoped<IObtenerRegistroVacunoReporteUseCase, ObtenerRegistroVacunoReporteUseCase>();

        return services;
    }
}
