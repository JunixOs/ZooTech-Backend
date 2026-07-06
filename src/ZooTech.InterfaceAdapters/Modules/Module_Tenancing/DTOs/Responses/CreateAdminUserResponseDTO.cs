namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Responses
{
    public class CreateAdminUserResponseDTO
    {
        public string Email { get; set; } = default!;
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
    }
}