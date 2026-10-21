using ZooTech.Application.Modules.Module_GestionUsuarios.UseCases.RegularUser.Login;
using ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios.DTOs;

namespace ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios.Mappers
{
    public class AuthRegularUserLoginMapper
    {
        public static LoginCommand ToCommand(AuthRegularUserLoginRequestDTO request)
        {
            return new LoginCommand
            {
                Email = request.Email
            };
        }
    }
}