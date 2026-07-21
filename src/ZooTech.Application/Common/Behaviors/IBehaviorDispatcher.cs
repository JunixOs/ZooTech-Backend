namespace ZooTech.Application.Common.Behaviors
{
    public interface IBehaviorDispatcher
    {
        Task<TResponse> Send<TRequest , TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default!
        );
    }
}