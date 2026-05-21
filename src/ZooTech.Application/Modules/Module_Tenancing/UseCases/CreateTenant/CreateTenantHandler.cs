using MediatR;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant.Ports;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases
{
    public class CreateTenantHandler
        : IRequestHandler<CreateTenantCommand, Unit>
    {
        private readonly ICreateTenantInputPort
            _inputPort;

        public CreateTenantHandler(
            ICreateTenantInputPort inputPort)
        {
            _inputPort = inputPort;
        }

        public async Task<Unit> Handle(
            CreateTenantCommand request,
            CancellationToken cancellationToken)
        {
            await _inputPort.Handle(request);

            return Unit.Value;
        }
    }
}
