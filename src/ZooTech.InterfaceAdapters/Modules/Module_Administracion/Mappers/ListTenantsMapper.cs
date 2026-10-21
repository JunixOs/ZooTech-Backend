using ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.ListTenants;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Administracion.Mappers
{
    public static class ListTenantsMapper
    {
        public static List<ListTenantsResponseDTO> ToResponse(List<ListTenantsOutput> output)
            => output.Select(o => new ListTenantsResponseDTO(
                o.Id,
                o.Code,
                o.SubDomain,
                o.LegalName,
                o.Email,
                o.Phone,
                o.Status,
                o.CreatedAt
            )).ToList();
    }
}