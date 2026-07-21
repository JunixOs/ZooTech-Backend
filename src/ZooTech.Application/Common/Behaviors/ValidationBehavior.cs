using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
        : IBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<ICommandValidator<TRequest>> _validators;

        public ValidationBehavior(
            IEnumerable<ICommandValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            Func<Task<TResponse>> next)
        {
            List<string> errors = new();

            foreach (var validator in _validators)
            {
                errors.AddRange(validator.Validate(request));

                if (errors.Count != 0)
                {
                    throw new ValidationException(
                        errors,
                        ScopeName.Application,
                        validator.ModuleName);
                }
            }

            return await next();
        }
    }
}
