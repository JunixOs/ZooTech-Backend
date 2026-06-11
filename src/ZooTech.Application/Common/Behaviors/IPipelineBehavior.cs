using MediatR;

namespace ZooTech.Application.Common.Behaviors
{
    public interface IPipelineBehavior<TRequest , TResponse>
    {
        Task<TResponse> Handle(
            TRequest request , 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken
        );
    }
}