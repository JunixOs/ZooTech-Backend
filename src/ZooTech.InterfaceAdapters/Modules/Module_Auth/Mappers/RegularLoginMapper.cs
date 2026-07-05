using ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin;
using ZooTech.InterfaceAdapters.Modules.Module_Auth.DTOs;

namespace ZooTech.InterfaceAdapters.Modules.Module_Auth.Mappers
{
    public class RegularLoginMapper
    {
        public static RegularLoginCommand ToCommand(RegularLoginRequestDTO request)
        {
            return new RegularLoginCommand
            {
                Email = request.Email
            };
        }
    }
}