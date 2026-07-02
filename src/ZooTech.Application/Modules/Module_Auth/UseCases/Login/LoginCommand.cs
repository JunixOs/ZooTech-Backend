namespace ZooTech.Application.Modules.Module_Auth.UseCases
{
    public class LoginCommand
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}