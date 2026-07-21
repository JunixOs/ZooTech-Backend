using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers
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