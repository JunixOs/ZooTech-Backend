namespace ZooTech.Domain.Shared.Enums
{
    public enum UserRole
    {
        Admin,
        Regular
    }

    public static class AuthorizationRoles
    {
        public const string Admin = nameof(Admin);
        public const string Regular = nameof(Regular);
    }
}