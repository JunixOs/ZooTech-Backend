using ZooTech.Application.Modules.Module_Auth.UseCases.AdminLogin;

namespace ZooTech.Application.Common.Behaviors.Module_Auth.AdminLogin
{
    public class AdminLoginBehaviorPipelineFactory : IAdminLoginBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<AdminLoginCommand , string> _validation;
        private readonly LoggingBehavior<AdminLoginCommand, string> _logging;
        private readonly AuditBehavior<AdminLoginCommand, string> _audit;

        private readonly IAdminLoginInputPort _handler;

        public AdminLoginBehaviorPipelineFactory(
            ValidationBehavior<AdminLoginCommand , string> validation,
            LoggingBehavior<AdminLoginCommand, string> logging,
            AuditBehavior<AdminLoginCommand, string> audit,

            IAdminLoginInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<AdminLoginCommand, string> Create()
        {
            return new BehaviorPipeline<AdminLoginCommand, string>(
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