namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant
{
    public class CreateUserInTenantOutput
    {
        public string Email { get; set; } = default!;
        public string? UserName { get; set; }
        public string? FullName { get; set; }
    }
}