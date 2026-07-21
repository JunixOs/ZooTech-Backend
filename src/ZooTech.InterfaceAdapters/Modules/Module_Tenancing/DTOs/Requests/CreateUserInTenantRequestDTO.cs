namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests
{
    public class CreateUserInTenantRequestDTO
    {
        public string? Code { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }

        public int? TenantId { get; set; }
    }
}