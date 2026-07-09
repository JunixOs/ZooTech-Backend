namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public class CreateTenantOutput
    {
        public string Code { get; init; } = default!;
        public string SubDomain { get; init; } = default!;
        public string DisplayName { get; init; } = default!;
        public string LegalName { get; init; } = default!;
    }
}