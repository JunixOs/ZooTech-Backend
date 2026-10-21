using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateUserInTenant;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Administracion.Mappers
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