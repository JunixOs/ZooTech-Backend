using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

namespace ZooTech.Application.Common.Behaviors.Module_Vacuno.CreateVacuno
{
    public class CreateVacunoBehaviorPipelineFactory : ICreateVacunoBehaviorPipelineFactory
    {
        private readonly ValidationBehavior<CreateVacunoCommand , CreateVacunoOutput> _validation;
        private readonly LoggingBehavior<CreateVacunoCommand , CreateVacunoOutput> _logging;
        private readonly AuditBehavior<CreateVacunoCommand , CreateVacunoOutput> _audit;
        private readonly EvictCacheBehavior<CreateVacunoCommand, CreateVacunoOutput> _evictCache;

        private readonly ICreateVacunoInputPort _handler;

        public CreateVacunoBehaviorPipelineFactory(
            ValidationBehavior<CreateVacunoCommand , CreateVacunoOutput> validation,
            LoggingBehavior<CreateVacunoCommand , CreateVacunoOutput> logging,
            AuditBehavior<CreateVacunoCommand , CreateVacunoOutput> audit,
            EvictCacheBehavior<CreateVacunoCommand, CreateVacunoOutput> evictCache,
            ICreateVacunoInputPort handler
        )
        {
            _validation = validation;
            _logging = logging;
            _audit = audit;
            _evictCache = evictCache;

            _handler = handler;
        }

        public BehaviorPipeline<CreateVacunoCommand , CreateVacunoOutput> Create()
        {
            return new BehaviorPipeline<CreateVacunoCommand , CreateVacunoOutput>(
            [
                _validation,
                _logging,
                _audit,
                _evictCache
            ],
            _handler.HandleAsync
            );
        }
    }
}