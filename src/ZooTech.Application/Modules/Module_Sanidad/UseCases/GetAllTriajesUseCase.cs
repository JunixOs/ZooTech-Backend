using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class GetAllTriajesUseCase
{
    private readonly ITriajeRepository _repository;

    public GetAllTriajesUseCase(ITriajeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResponse<TriajeResponse>> ExecuteAsync(
        int pagina,
        int tamano,
        string? fecha = null,
        string? codigo = null,
        string? nombre = null,
        string? tipoPeso = null,
        decimal? pesoKg = null)
    {
        var (triajes, total) = await _repository.GetAllAsync(
            pagina, tamano, fecha, codigo, nombre, tipoPeso, pesoKg);

        var items = triajes.Select(t => new TriajeResponse
        {
            Id = t.Id,
            Codigo = t.Codigo,
            FechaHora = t.FechaHora,
            VacunoId = t.VacunoId,
            VacunoNombre = t.VacunoNombre,
            TipoPesoCode = t.TipoPesoCode,
            PesoKg = t.PesoKg,
            Observaciones = t.Observaciones,
            EstadoRegistroCode = t.EstadoRegistroCode,
            EncargadoUsuarioId = t.EncargadoUsuarioId,
            CreatedAt = t.CreatedAt
        });

        return new PagedResponse<TriajeResponse>
        {
            Data = items,
            TotalRegistros = total,
            Pagina = pagina,
            Tamano = tamano,
            TotalPaginas = (int)Math.Ceiling((double)total / tamano)
        };
    }
}