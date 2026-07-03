namespace ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin
{
    public interface IRegularLoginInputPort
    {
        Task<string> Handle(string email);
    }
}