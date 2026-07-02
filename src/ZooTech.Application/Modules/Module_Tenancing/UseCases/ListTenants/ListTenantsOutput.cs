namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants
{
    public class ListTenantsOutput
    {
        string Code { get; set; } = default!;
        string SubDomain { get; set; } = default!;
        string LegalName { get; set; } = default!;
        string Email { get; set; } = default!;
        string Phone { get; set; } = default!;
        string Status { get; set; } = default!;
        DateTime CreatedAt { get; set; } = default!;
    }
}