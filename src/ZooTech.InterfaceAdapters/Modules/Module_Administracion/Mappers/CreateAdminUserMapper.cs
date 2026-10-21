using ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.CreateAdminUser;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Administracion.Mappers
{
    public class CreateAdminUserMapper
    {
        public static CreateAdminUserCommand ToCommand(CreateAdminUserRequestDTO dto)
        {
            return new CreateAdminUserCommand
            {
                Email = dto.Email,
                UserName = dto.UserName,
                Password = dto.Password,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                IsActive = dto.IsActive,
                Metadata = dto.Metadata
            };
        }

        public static CreateAdminUserResponseDTO ToResponseDto(CreateAdminUserOutput output)
        {
            return new CreateAdminUserResponseDTO
            {
                Email = output.Email,
                UserName = output.UserName,
                FirstName = output.FirstName
            };
        }
    }
}