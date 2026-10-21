namespace ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Responses
{
    public class CreateUserInTenantResponseDTO
    {
        public string Email { get; set; } = default!;
        public string? UserName { get; set; }
        public string? FullName { get; set; }
    }
}