using ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin;
using ZooTech.Domain.Shared.ValueObjects;
using ZooTech.InterfaceAdapters.Modules.Module_Auth.DTOs;

namespace ZooTech.InterfaceAdapters.Modules.Module_Auth.Mappers
{
    public class AdminLoginMapper
    {
        public static AdminLoginCommand ToCommand(AdminLoginRequest request)
        {
            return new AdminLoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };
        }
    }
}