using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Validator
{
    public interface ICommandValidator<TRequest>
    {
        ModuleName ModuleName { get; }
        List<string> Validate(TRequest request);
    }
}