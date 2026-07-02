using MediatR;
using Microsoft.Extensions.Logging;

namespace ZooTech.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            Func<Task<TResponse>> next
        )
        {
            _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
            var response = await next();
            _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
            return response;
        }
    }
}
