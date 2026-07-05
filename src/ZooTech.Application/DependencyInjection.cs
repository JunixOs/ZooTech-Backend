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
