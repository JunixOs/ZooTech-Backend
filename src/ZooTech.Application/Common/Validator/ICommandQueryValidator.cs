using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Common.Validator
{
    public interface ICommandQueryValidator<TRequest>
    {
        ModuleName ModuleName { get; }
        List<string> Validate(TRequest request);
    }
}