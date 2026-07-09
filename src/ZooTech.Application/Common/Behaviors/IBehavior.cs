namespace ZooTech.Application.Common.Behaviors
{
    public interface IBehavior<TRequest , TResponse>
    {
        Task<TResponse> Handle(
            TRequest request,
            Func<Task<TResponse>> next
        );
    }
}