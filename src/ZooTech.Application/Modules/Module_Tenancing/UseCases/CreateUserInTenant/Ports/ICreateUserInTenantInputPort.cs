using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant.Ports
{
    public interface ICreateUserInTenantInputPort : IRequestHandler<CreateUserInTenantCommand , CreateUserInTenantOutput>
    {
    }
}