using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

public sealed class GetAllTriajesInteractor : IGetAllTriajesInputPort
{
    private readonly ITriajeRepository _repository;

    public GetAllTriajesInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllTriajesOutput> HandleAsync(GetAllTriajesQuery query, CancellationToken cancellationToken = default)
    {
        var pagina = query.Pagina <= 0 ? 1 : query.Pagina;
        var tamano = query.Tamano <= 0 ? 10 : Math.Min(query.Tamano, 100);

        var (triajes, total) = await _repository.GetAllAsync(
            pagina, tamano, query.Fecha, query.Codigo, query.Nombre, query.TipoPeso, query.PesoKg, cancellationToken);

        var items = triajes.Select(t => new TriajeItemOutput(
            t.Id, t.Codigo, t.FechaHora, t.VacunoId, t.VacunoNombre,
            t.TipoPesoCode, t.PesoKg, t.Observaciones, t.EstadoRegistroCode,
            t.EncargadoUsuarioId, t.CreatedAt)).ToList();

        var totalPaginas = (int)Math.Ceiling((double)total / tamano);

        return new GetAllTriajesOutput(items, total, pagina, tamano, totalPaginas);
    }
}
