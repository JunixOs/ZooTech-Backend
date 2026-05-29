using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Modules.Module_Celo.UseCases.ListarCelos;
using ZooTech.Application.Modules.Module_Sanidad.UseCases;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;

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
        services.AddScoped<GetAllTipoPesosUseCase>();
        services.AddScoped<GetAllVacunosUseCase>();
        services.AddScoped<GetHistorialByVacunoIdUseCase>();


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
        // Use Cases - Module_ProduccionLeche
        // ============================================

        services.AddScoped<ICreateOrdenioInputPort, CreateOrdenioInteractor>();
        services.AddScoped<IGetOrdenioByIdInputPort, GetOrdenioByIdInteractor>();
        services.AddScoped<IListOrdeniosInputPort, ListOrdeniosInteractor>();
        services.AddScoped<IUpdateOrdenioInputPort, UpdateOrdenioInteractor>();
        services.AddScoped<IDeleteOrdenioInputPort, DeleteOrdenioInteractor>();

        services.AddScoped<IListarCelosUseCase, ListarCelosUseCase>();

        return services;
    }
}