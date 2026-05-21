namespace ZooTech.Application.Common.Validator
{
    public interface IValidator<T>
    {
        void Validate(T userRequest);
    }
}