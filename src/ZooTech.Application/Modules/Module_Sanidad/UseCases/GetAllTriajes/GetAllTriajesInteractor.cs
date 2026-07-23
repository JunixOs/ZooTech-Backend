using ZooTech.Application.Modules.Module_Sanidad.UseCases.Common;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTriajes;

public sealed class GetAllTriajesInteractor : IGetAllTriajesInputPort
{
    private readonly ITriajeRepository _repository;

    public GetAllTriajesInteractor(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllTriajesOutput> Handle(GetAllTriajesQuery query, CancellationToken cancellationToken = default)
    {
        var pagina = query.Pagina <= 0 ? 1 : query.Pagina;
        var tamano = query.Tamano <= 0 ? 10 : Math.Min(query.Tamano, 100);

        var (triajes, total) = await _repository.GetAllAsync(
            pagina: pagina,
            tamano: tamano,
            fecha: query.Fecha,
            fechaDesde: query.FechaDesde,
            fechaHasta: query.FechaHasta,
            codigo: query.Codigo,
            nombre: query.Nombre,
            tipoPeso: query.TipoPeso,
            pesoKg: query.PesoKg,
            vacunoId: query.VacunoId,
            uniqueVacuno: query.UniqueVacuno,
            cancellationToken: cancellationToken
        );

        var items = triajes.Select(t => new TriajeOutput(
            t.Id, t.Codigo, t.FechaHora, t.VacunoId, t.VacunoNombre,
            t.TipoPesoCode, t.PesoKg, t.Observaciones, t.EstadoRegistroCode,
            t.EncargadoUsuarioId, t.CreatedAt)).ToList();

        return new GetAllTriajesOutput(items, total);
    }
}