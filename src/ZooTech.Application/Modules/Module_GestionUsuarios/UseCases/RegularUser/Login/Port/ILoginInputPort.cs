using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_GestionUsuarios.UseCases.RegularUser.Login.Port
{
    public interface ILoginInputPort : IRequestHandler<LoginCommand , string>
    {
    }
}