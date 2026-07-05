namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests
{
    public class CreateAdminUserRequestDTO
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public string? Metadata { get; set; }
    }
}