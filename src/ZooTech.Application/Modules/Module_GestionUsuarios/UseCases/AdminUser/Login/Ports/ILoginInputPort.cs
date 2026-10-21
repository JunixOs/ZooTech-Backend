using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_GestionUsuarios.UseCases.AdminUser.Login.Ports
{
    public interface ILoginInputPort 
        : IRequestHandler<LoginCommand , string>
    {
    }
}