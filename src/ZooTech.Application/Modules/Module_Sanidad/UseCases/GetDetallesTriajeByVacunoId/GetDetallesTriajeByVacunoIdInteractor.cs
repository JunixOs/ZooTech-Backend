using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetDetalleTriajeByVacunoId
{
    public sealed class GetDetallesTriajeByVacunoIdInteractor : IGetDetallesTriajeByVacunoIdInputPort
    {
        private readonly ITriajeRepository _repository;

        public GetDetallesTriajeByVacunoIdInteractor(ITriajeRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetDetallesTriajeByVacunoIdOutput> HandleAsync(GetDetallesTriajeByVacunoIdQuery query, CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetDetallesByVacunoIdAsync(query.VacunoId, cancellationToken);
            return new GetDetallesTriajeByVacunoIdOutput(items.ToList());
        }
    }
}