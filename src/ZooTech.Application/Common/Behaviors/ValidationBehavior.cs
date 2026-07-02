using ZooTech.Application.Common.Exceptions;
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
            List<string> errors = _validator.Validate(request);

            if(errors.Count != 0)
            {
                throw new ValidationException(_validator.ModuleName.ToString(), errors);
            }

            return await next();
        }
    }
}
