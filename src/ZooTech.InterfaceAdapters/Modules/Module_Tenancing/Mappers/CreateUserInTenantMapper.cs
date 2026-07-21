using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers
{
    public class CreateUserInTenantMapper
    {
        public static CreateUserInTenantResponseDTO ToResponse(CreateUserInTenantOutput output)
        {
            return new CreateUserInTenantResponseDTO
            {
                Email = output.Email,
                UserName = output.UserName,
                FullName = output.FullName
            };
        }

        public static CreateUserInTenantCommand ToCommand(CreateUserInTenantRequestDTO dto)
        {
            return new CreateUserInTenantCommand
            {
                Code = dto.Code,
                UserName = dto.UserName,
                FullName = dto.FullName,
                Email = dto.Email,
                IsActive = dto.IsActive,
                TenantId = dto.TenantId
            };
        }
    }
}