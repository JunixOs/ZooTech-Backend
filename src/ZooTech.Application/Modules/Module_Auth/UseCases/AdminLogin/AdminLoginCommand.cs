namespace ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin
{
    public class AdminLoginCommand
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}