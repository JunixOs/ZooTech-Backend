using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Gateway.Repositories.MainTenantsDb;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers.Ports;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers
{
    public class ListAdminUsersInteractor : IListAdminUsersInputPort, IAuditableRequest
    {
        public AuditEventType EventType => AuditEventType.Read;

        public string Action => "List admin users";

        private readonly IAdminUserRepository _adminUserRepository;

        public ListAdminUsersInteractor(
            IAdminUserRepository adminUserRepository
        )
        {
            _adminUserRepository = adminUserRepository;
        }

        public Task<List<ListAdminUsersOutput>> Handle(EmptyCommand emptyCmd)
        {
            return _adminUserRepository.FindAll();
        }
    }
}