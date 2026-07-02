namespace ZooTech.Application.Modules.Module_Auth.UseCases
{
    public interface ILoginInputPort
    {
        Task<string> Handle(LoginCommand cmd);
    }
}