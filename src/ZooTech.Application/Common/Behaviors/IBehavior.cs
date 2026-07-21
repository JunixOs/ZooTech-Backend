namespace ZooTech.Application.Common.Behaviors
{
    // De esta interfaz heredan todos los Behaviors que se pueden añadir
    // al pipeline como Validation, Caching, Audit, etc.
    public interface IBehavior<TRequest , TResponse>
    {
        Task<TResponse> Handle(
            TRequest request,
            Func<Task<TResponse>> next
        );
    }
}