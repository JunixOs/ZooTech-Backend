using Microsoft.Extensions.DependencyInjection;

namespace ZooTech.Application.Common.Behaviors
{
    public class BehaviorDispatcher : IBehaviorDispatcher
    {
        // Interfaz principal para acceder al Contenedor de Inyeccion de Dependencias,
        // aqui sirve para obtener un servicio de forma dinamica sin tener que declararlo en el
        // constructor
        private readonly IServiceProvider _provider;

        public BehaviorDispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }
        
        public async Task<TResponse> Send<TRequest, TResponse>(
            TRequest request, 
            CancellationToken cancellationToken = default
        )
        {
            // Obtiene los Behaviors para esta peticion especifica
            var behaviors = _provider.GetServices<IBehavior<TRequest , TResponse>>();

            // Obtiene el handler para esta peticion especifica
            var handler = _provider.GetRequiredService<IRequestHandler<TRequest , TResponse>>();

            // Arma el Pipeline con estos valores
            var pipeline = new BehaviorPipeline<TRequest , TResponse>(
                behaviors,
                handler.HandleAsync
            ); 
            
            // Ejecuta el pipeline con el request dado
            return await pipeline.Execute(
                request,
                cancellationToken
            );
        }
    }
}