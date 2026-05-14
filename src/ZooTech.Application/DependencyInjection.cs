using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;

namespace ZooTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {

        // ============================================
        // Use Cases - Module_Sanidad
        // ============================================
        services.AddScoped<GetAllTriajesUseCase>();
        services.AddScoped<GetTriajeByIdUseCase>();
        services.AddScoped<CreateTriajeUseCase>();
        services.AddScoped<UpdateTriajeUseCase>();
        services.AddScoped<DeleteTriajeUseCase>();


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

        return services;
    }
}