namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants
{
    public class ListTenantsOutput
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string SubDomain { get; set; } = default!;
        public string LegalName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime? CreatedAt { get; set; } = default!;
    }
}