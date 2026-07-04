using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Behaviors.Module_Auth.AdminLogin;
using ZooTech.Application.Common.Behaviors.Module_Auth.RegularLogin;
using ZooTech.Application.Common.Behaviors.Module_Tenancing;
using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin;
using ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;

namespace ZooTech.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddTransient(typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(AuditBehavior<,>));

        services.AddScoped<IAdminLoginBehaviorPipeline, AdminLoginBehaviorPipeline>();
        services.AddScoped<IRegularLoginBehaviorPipeline, RegularLoginBehaviorPipeline>();
        services.AddScoped<ICreateTenantPipelineFactory, CreateTenantPipelineFactory>();

        services.AddScoped<IAdminLoginInputPort, AdminLoginInteractor>();
        services.AddScoped<IRegularLoginInputPort, RegularLoginInteractor>();
        services.AddScoped<ICreateTenantInputPort, CreateTenantInteractor>();

        services.AddScoped<ICommandValidator<AdminLoginCommand>, AdminLoginValidator>();
        services.AddScoped<ICommandValidator<string>, RegularLoginValidator>();
        services.AddScoped<ICommandValidator<CreateTenantCommand>, CreateTenantValidation>();

        return services;
    }
}
