namespace ZooTech.Application.Common.Validator
{
    public interface ICommandValidator<TRequest>
    {
        List<string> Validate(TRequest request);
    }
}