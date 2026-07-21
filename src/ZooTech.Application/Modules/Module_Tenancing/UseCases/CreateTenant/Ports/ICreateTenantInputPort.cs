using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public interface ICreateTenantInputPort : IRequestHandler<CreateTenantCommand , CreateTenantOutput>
    {
    }
}