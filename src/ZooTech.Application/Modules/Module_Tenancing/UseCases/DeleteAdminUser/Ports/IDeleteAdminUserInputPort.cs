using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser.Ports
{
    public interface IDeleteAdminUserInputPort
    {
        Task<EmptyOutput> Handle(DeleteAdminUserCommand cmd);
    }
}