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

                // Aqui se busca la interfaz asociada al Interactor o UseCase
                // Solo acepta si la interfaz termina en InputPort o UseCase
                var inputPort = implementation.GetInterfaces()
                    .FirstOrDefault(i =>
                        i.Name.EndsWith("InputPort") ||
                        i.Name.EndsWith("UseCase")
                    );

                if (inputPort == null)
                    continue;

                services.AddScoped(inputPort, implementation);

                // Aqui se obtiene la interfaz asociada al InputPort
                // IRequestHandler<,>  de cualquier tipo
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