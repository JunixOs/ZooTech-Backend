using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.ListTenants.Ports
{
    public interface IListTenantsInputPort : IRequestHandler<ListTenantsQuery , List<ListTenantsOutput>>
    {
    }
}