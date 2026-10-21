using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateTenant;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Administracion.Mappers
{
    public static class CreateTenantMapper
    {
        public static CreateTenantCommand ToCommand(CreateTenantRequestDto dto)
        {
            return new CreateTenantCommand
            {
                Code = dto.Code,
                SubDomain = dto.SubDomain,
                DisplayName = dto.DisplayName,
                LegalName = dto.LegalName,
                Email = dto.Email,
                Phone = dto.Phone,
                TimeZone = dto.TimeZone,
                TenantAddress = new TenantAddress
                {
                    Country = dto.TenantAddress.Country,
                    State = dto.TenantAddress.State,
                    Province = dto.TenantAddress.Province,
                    City = dto.TenantAddress.City,
                    AddressLine_1 = dto.TenantAddress.AddressLine1,
                    AddressLine_2 = dto.TenantAddress.AddressLine2 ?? string.Empty
                },
                TenantBranding = new TenantBranding
                {
                    PrimaryColor = dto.TenantBranding.PrimaryColor,
                    SecondaryColor = dto.TenantBranding.SecondaryColor,
                    LogoUrl = dto.TenantBranding.LogoUrl
                },
                TenantDatabaseConnection = new TenantDatabaseConnection
                {
                    IsActive = dto.TenantDatabaseConnection.IsActive
                }
            };
        }

        public static CreateTenantResponseDto ToResponseDto(CreateTenantOutput output)
        {
            return new CreateTenantResponseDto
            {
                Code = output.Code,
                SubDomain = output.SubDomain,
                DisplayName = output.DisplayName,
                LegalName = output.LegalName
            };
        }
    }
}
