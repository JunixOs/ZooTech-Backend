using FluentValidation;
using MediatR;
using ZooTech.Application.Common.Validator;

namespace ZooTech.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IBehavior<TRequest , TResponse>
    {
        private readonly ICommandValidator<TRequest> _validator;
        public ValidationBehavior(
            ICommandValidator<TRequest> validator
        )
        {
            _validator = validator;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            Func<Task<TResponse>> next
        )
        {
            _validator.Validate(request);

            return await next();
        }
    }
}
