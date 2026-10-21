namespace ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateTenant
{
    public class CreateTenantOutput
    {
        public string Code { get; init; } = default!;
        public string SubDomain { get; init; } = default!;
        public string DisplayName { get; init; } = default!;
        public string LegalName { get; init; } = default!;
    }
}