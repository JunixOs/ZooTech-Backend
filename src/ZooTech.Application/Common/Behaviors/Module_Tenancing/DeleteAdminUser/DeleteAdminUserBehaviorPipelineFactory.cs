using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser.Ports;

namespace ZooTech.Application.Common.Behaviors.Module_Tenancing.DeleteAdminUser
{
    public class DeleteAdminUserBehaviorPipelineFactory : IDeleteAdminUserBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<DeleteAdminUserCommand, EmptyOutput> _validation;
        private readonly LoggingBehavior<DeleteAdminUserCommand, EmptyOutput> _logging;
        private readonly AuditBehavior<DeleteAdminUserCommand, EmptyOutput> _audit;

        private readonly IDeleteAdminUserInputPort _handler;

        public DeleteAdminUserBehaviorPipelineFactory(
            ValidationBehavior<DeleteAdminUserCommand, EmptyOutput> validation,
            LoggingBehavior<DeleteAdminUserCommand, EmptyOutput> logging,
            AuditBehavior<DeleteAdminUserCommand, EmptyOutput> audit,

            IDeleteAdminUserInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<DeleteAdminUserCommand, EmptyOutput> Create()
        {
            return new BehaviorPipeline<DeleteAdminUserCommand, EmptyOutput>(
            [
                _validation,
                _logging,
                _audit
            ],
            _handler.Handle
            );
        }
    }
}