namespace ZooTech.Application.Common.Behaviors
{
    public class BehaviorPipeline<TRequest , TResponse>
    {
        private readonly IEnumerable<IBehavior<TRequest , TResponse>> _behaviors;
        private readonly Func<TRequest, Task<TResponse>> _handler;

        public BehaviorPipeline(
            IEnumerable<IBehavior<TRequest , TResponse>> behaviors,
            Func<TRequest, Task<TResponse>> handler
        )
        {
            _behaviors = behaviors;
            _handler = handler;
        }

        public Task<TResponse> Execute(TRequest request)
        {
            Func<Task<TResponse>> next = () => _handler(request);

            foreach (var behavior in _behaviors.Reverse())
            {
                var current = next;
                next = () => behavior.Handle(request, current);
            }

            return next();
        }
    }
}