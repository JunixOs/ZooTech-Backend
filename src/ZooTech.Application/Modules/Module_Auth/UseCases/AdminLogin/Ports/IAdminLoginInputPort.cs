namespace ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin
{
    public interface IAdminLoginInputPort
    {
        Task<string> Handle(AdminLoginCommand cmd);
    }
}