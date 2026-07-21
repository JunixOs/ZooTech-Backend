using Microsoft.Extensions.DependencyInjection;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin;

namespace ZooTech.Application.DependencyInjection
{
    public static class InteractorRegistrationExtensions
    {
        public static IServiceCollection AddInteractors(
            this IServiceCollection services
        )
        {
            var assembly = typeof(AdminLoginInteractor).Assembly;

            foreach (var implementation in assembly.GetTypes())
            {
                if (implementation.IsAbstract || implementation.IsInterface)
                    continue;

                var inputPort = implementation.GetInterfaces()
                    .FirstOrDefault(i =>
                        i.Name.EndsWith("InputPort"));

                if (inputPort == null)
                    continue;

                services.AddScoped(inputPort, implementation);

                var requestHandler = inputPort.GetInterfaces()
                    .FirstOrDefault(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

                if (requestHandler != null)
                {
                    services.AddScoped(requestHandler,
                        sp => sp.GetRequiredService(inputPort));
                }
            }

            return services;
        }
    }
}