using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers
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