using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Modules.Module_Celo.UseCases.CreateCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.DeleteTriaje;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialByVacunoId;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetTriajeById;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.DeleteVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.CreateOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.DeleteOrdenio;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosPdf;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GenerateOrdeniosExcel;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.GetOrdenioById;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.ListOrdenios;
using ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.Ordenios.UpdateOrdenio;
using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Behaviors.Module_Auth.AdminLogin;
using ZooTech.Application.Common.Behaviors.Module_Auth.RegularLogin;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateAdminUser;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateTenant;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateUserInTenant;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.DeleteAdminUser;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.ListAdminUsers;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin;
using ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin;
using ZooTech.Application.Modules.Module_Tenancing.UseCases;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser.Ports;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant.Ports;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser.Ports;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers.Ports;

namespace ZooTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // ============================================
        // Use Cases - Module_Sanidad
        // ============================================
        services.AddScoped<IGetAllTriajesInputPort, GetAllTriajesInteractor>();
        services.AddScoped<IGetTriajeByIdInputPort, GetTriajeByIdInteractor>();
        services.AddScoped<ICreateTriajeInputPort, CreateTriajeInteractor>();
        services.AddScoped<IUpdateTriajeInputPort, UpdateTriajeInteractor>();
        services.AddScoped<IDeleteTriajeInputPort, DeleteTriajeInteractor>();
        services.AddScoped<IGetAllTipoPesosInputPort, GetAllTipoPesosInteractor>();
        services.AddScoped<IGetAllVacunosSanidadInputPort, GetAllVacunosSanidadInteractor>();
        services.AddScoped<IGetHistorialByVacunoIdInputPort, GetHistorialByVacunoIdInteractor>();

        // ============================================
        // Use Cases - Module_ProduccionLeche
        // ============================================
        services.AddScoped<ICreateOrdenioInputPort, CreateOrdenioInteractor>();
        services.AddScoped<IGetOrdeniosPdfInputPort, GenerateOrdeniosPdfInteractor>();
        services.AddScoped<IGetOrdeniosExcelInputPort, GenerateOrdeniosExcelInteractor>();
        services.AddScoped<IGetOrdenioByIdInputPort, GetOrdenioByIdInteractor>();
        services.AddScoped<IListOrdeniosInputPort, ListOrdeniosInteractor>();
        services.AddScoped<IUpdateOrdenioInputPort, UpdateOrdenioInteractor>();
        services.AddScoped<IDeleteOrdenioInputPort, DeleteOrdenioInteractor>();

        // ============================================
        // FluentValidation — all assemblies
        // ============================================
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        // ============================================
        // Use Cases - Module_Celo
        // ============================================
        services.AddScoped<IGetCelosInputPort, GetCelosInteractor>();
        services.AddScoped<ICreateCeloInputPort, CreateCeloInteractor>();
        services.AddScoped<IUpdateCeloInputPort, UpdateCeloInteractor>();
        services.AddScoped<IDeleteCeloInputPort, DeleteCeloInteractor>();

        // ============================================
        // Use Cases - Module_Vacuno
        // ============================================
        services.AddScoped<IListarVacunosInputPort, ListarVacunosInteractor>();
        services.AddScoped<ICreateVacunoInputPort, CreateVacunoInteractor>();
        services.AddScoped<IGetVacunoByIdInputPort, GetVacunoByIdInteractor>();
        services.AddScoped<IUpdateVacunoInputPort, UpdateVacunoInteractor>();
        services.AddScoped<IDeleteVacunoInputPort, DeleteVacunoInteractor>();
        services.AddTransient(typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(AuditBehavior<,>));

        services.AddScoped<IAdminLoginBehaviorPipelineFactory, AdminLoginBehaviorPipelineFactory>();
        services.AddScoped<IRegularLoginBehaviorPipelineFactory, RegularLoginBehaviorPipelineFactory>();

        services.AddScoped<ICreateTenantBehaviorPipelineFactory, CreateTenantBehaviorPipelineFactory>();
        services.AddScoped<ICreateUserInTenantBehaviorPipelineFactory, CreateUserInTenantBehaviorPipelineFactory>();

        services.AddScoped<IDeleteAdminUserBehaviorPipelineFactory, DeleteAdminUserBehaviorPipelineFactory>();
        services.AddScoped<ICreateAdminUserBehaviorPipelineFactory, CreateAdminUserBehaviorPipelineFactory>();
        services.AddScoped<IListAdminUsersBehaviorPipelineFactory, ListAdminUsersBehaviorPipelineFactory>();

        services.AddScoped<IAdminLoginInputPort, AdminLoginInteractor>();
        services.AddScoped<IRegularLoginInputPort, RegularLoginInteractor>();
        
        services.AddScoped<ICreateTenantInputPort, CreateTenantInteractor>();
        services.AddScoped<ICreateUserInTenantInputPort, CreateUserInTenantInteractor>();
        
        services.AddScoped<IDeleteAdminUserInputPort, DeleteAdminUserInteractor>();
        services.AddScoped<ICreateAdminUserInputPort , CreateAdminUserInteractor>();
        services.AddScoped<IListAdminUsersInputPort, ListAdminUsersInteractor>();
        
        services.AddScoped<ICommandValidator<AdminLoginCommand>, AdminLoginValidator>();
        services.AddScoped<ICommandValidator<RegularLoginCommand>, RegularLoginValidator>();
        
        services.AddScoped<ICommandValidator<CreateTenantCommand>, CreateTenantValidation>();
        services.AddScoped<ICommandValidator<CreateUserInTenantCommand>, CreateUserInTenantValidator>();
        
        services.AddScoped<ICommandValidator<DeleteAdminUserCommand>, DeleteAdminUserValidator>();
        services.AddScoped<ICommandValidator<CreateAdminUserCommand> , CreateAdminUserValidator>();

        return services;
    }
}
