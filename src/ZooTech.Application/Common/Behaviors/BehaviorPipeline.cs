namespace ZooTech.Application.Common.Behaviors
{
    // Clase que sirve para construir un Pipeline que incluya todos los Behaviors que se indique 
    public class BehaviorPipeline<TRequest , TResponse>
    {
        private readonly IEnumerable<IBehavior<TRequest , TResponse>> _behaviors;
        
        // NOTA: Aqui modifique para que tambien reciba el CancelationToken, asi es mas seguro creo :)
        private readonly Func<TRequest, CancellationToken, Task<TResponse>> _handler;

        public BehaviorPipeline(
            IEnumerable<IBehavior<TRequest , TResponse>> behaviors,
            Func<TRequest, CancellationToken, Task<TResponse>> handler
        )
        {
            _behaviors = behaviors;
            _handler = handler;
        }

        public Task<TResponse> Execute(
            TRequest request,
            CancellationToken cancellationToken
        )
        {
            Func<Task<TResponse>> next = () => _handler(request, cancellationToken);

            foreach (var behavior in _behaviors.Reverse())
            {
                var current = next;
                next = () => behavior.Handle(request, current);
            }

            return next();
        }
    }
}