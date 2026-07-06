using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers.Ports
{
    public interface IListAdminUsersInputPort
    {
        Task<List<ListAdminUsersOutput>> Handle(EmptyCommand emptyCmd);
    }
}