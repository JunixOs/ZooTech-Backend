namespace ZooTech.Application.Modules.Module_Administracion.UseCases.TenantManagement.CreateUserInTenant
{
    public class CreateUserInTenantOutput
    {
        public string Email { get; set; } = default!;
        public string? UserName { get; set; }
        public string? FullName { get; set; }
    }
}