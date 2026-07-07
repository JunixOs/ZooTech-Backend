namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant.Ports
{
    public interface ICreateUserInTenantInputPort
    {
        Task<CreateUserInTenantOutput> Handle(CreateUserInTenantCommand cmd, CancellationToken cancellationToken = default);
    }
}