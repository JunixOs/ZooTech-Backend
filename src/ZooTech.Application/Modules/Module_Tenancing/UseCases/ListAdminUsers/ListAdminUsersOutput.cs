namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers
{
    public class ListAdminUsersOutput
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}