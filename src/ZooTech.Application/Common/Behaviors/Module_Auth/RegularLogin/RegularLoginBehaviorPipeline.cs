using ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin;

namespace ZooTech.Application.Common.Behaviors.Module_Auth.RegularLogin
{
    public class RegularLoginBehaviorPipeline : IRegularLoginBehaviorPipeline
    {
        private readonly ValidationBehavior<string , string> _validation;
        private readonly LoggingBehavior<string, string> _logging;
        private readonly AuditBehavior<string, string> _audit;

        private readonly IRegularLoginInputPort _handler;

        public RegularLoginBehaviorPipeline(
            ValidationBehavior<string , string> validation,
            LoggingBehavior<string, string> logging,
            AuditBehavior<string, string> audit,

            IRegularLoginInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<string, string> Create()
        {
            return new BehaviorPipeline<string, string>(
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