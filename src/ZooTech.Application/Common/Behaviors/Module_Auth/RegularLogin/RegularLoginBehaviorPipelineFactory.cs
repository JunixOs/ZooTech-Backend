using ZooTech.Application.Modules.Module_Auth.UseCases.RegularLogin;

namespace ZooTech.Application.Common.Behaviors.Module_Auth.RegularLogin
{
    public class RegularLoginBehaviorPipelineFactory : IRegularLoginBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<RegularLoginCommand , string> _validation;
        private readonly LoggingBehavior<RegularLoginCommand, string> _logging;
        private readonly AuditBehavior<RegularLoginCommand, string> _audit;

        private readonly IRegularLoginInputPort _handler;

        public RegularLoginBehaviorPipelineFactory(
            ValidationBehavior<RegularLoginCommand , string> validation,
            LoggingBehavior<RegularLoginCommand, string> logging,
            AuditBehavior<RegularLoginCommand, string> audit,

            IRegularLoginInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;

            _handler = handler;
        }

        public BehaviorPipeline<RegularLoginCommand, string> Create()
        {
            return new BehaviorPipeline<RegularLoginCommand, string>(
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