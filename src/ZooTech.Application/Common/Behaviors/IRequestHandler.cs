namespace ZooTech.Application.Common.Behaviors
{
    public interface IRequestHandler<TRequest , TResponse>
    {
        Task<TResponse> HandleAsync(
            TRequest request,
            CancellationToken cancellationToken
        );
    }
}