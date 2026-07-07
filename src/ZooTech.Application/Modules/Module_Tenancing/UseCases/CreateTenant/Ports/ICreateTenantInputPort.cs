namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public interface ICreateTenantInputPort
    {
        Task<CreateTenantOutput> Handle(CreateTenantCommand request, CancellationToken cancellationToken = default);
    }
}