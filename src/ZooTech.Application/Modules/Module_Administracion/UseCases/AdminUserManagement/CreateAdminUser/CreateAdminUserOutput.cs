namespace ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.CreateAdminUser
{
    public class CreateAdminUserOutput
    {
        public string Email { get; set; } = default!;
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
    }
}