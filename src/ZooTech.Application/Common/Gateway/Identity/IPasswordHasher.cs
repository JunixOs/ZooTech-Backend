namespace ZooTech.Application.Common.Gateway.Identity
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Compare(string password, string hashedPassword);
    }
}