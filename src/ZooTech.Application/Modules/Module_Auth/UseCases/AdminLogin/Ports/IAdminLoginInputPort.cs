using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin
{
    public interface IAdminLoginInputPort 
        : IRequestHandler<AdminLoginCommand , string>
    {
    }
}