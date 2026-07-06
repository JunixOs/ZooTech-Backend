namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateAdminUser.Ports
{
    public interface ICreateAdminUserInputPort
    {
        Task<CreateAdminUserOutput> Handle(CreateAdminUserCommand cmd);
    }
}