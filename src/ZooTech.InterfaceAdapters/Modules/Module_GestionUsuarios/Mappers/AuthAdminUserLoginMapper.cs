using ZooTech.Application.Modules.Module_GestionUsuarios.UseCases.AdminUser.Login;
using ZooTech.Domain.Shared.ValueObjects;
using ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios.DTOs;

namespace ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios.Mappers
{
    public class AuthAdminUserLoginMapper
    {
        public static LoginCommand ToCommand(AuthAdminUserLoginRequestDTO request)
        {
            return new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };
        }
    }
}