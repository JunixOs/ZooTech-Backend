using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser.Ports
{
    public interface IDeleteAdminUserInputPort : IRequestHandler<DeleteAdminUserCommand , EmptyOutput>
    {
    }
}