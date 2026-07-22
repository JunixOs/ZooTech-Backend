namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser
{
    public class CreateAdminUserOutput
    {
        public string Email { get; set; } = default!;
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
    }
}