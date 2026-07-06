using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers.Ports;

namespace ZooTech.Application.Common.Behaviors.Module_Tenancing.ListAdminUsers
{
    public class ListAdminUsersBehaviorPipelineFactory : IListAdminUsersBehaviorPipelineFactory
    {
        private readonly LoggingBehavior<EmptyCommand, List<ListAdminUsersOutput>> _logging;
        private readonly AuditBehavior<EmptyCommand, List<ListAdminUsersOutput>> _audit;

        private readonly IListAdminUsersInputPort _handler;

        public ListAdminUsersBehaviorPipelineFactory(
            LoggingBehavior<EmptyCommand, List<ListAdminUsersOutput>> logging,
            AuditBehavior<EmptyCommand, List<ListAdminUsersOutput>> audit,

            IListAdminUsersInputPort handler
        )
        {
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<EmptyCommand, List<ListAdminUsersOutput>> Create()
        {
            return new BehaviorPipeline<EmptyCommand, List<ListAdminUsersOutput>>(
            [
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}