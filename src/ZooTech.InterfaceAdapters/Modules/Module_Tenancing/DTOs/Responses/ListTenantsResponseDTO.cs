namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Responses
{
    public sealed record ListTenantsResponseDTO(
        int Id,
        string Code,
        string SubDomain,
        string LegalName,
        string Email,
        string Phone,
        string Status,
        DateTime? CreatedAt
    );
}