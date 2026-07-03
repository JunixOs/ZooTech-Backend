using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Application.Modules.Module_Sanidad.Services;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public class DownloadReporteTriajesUseCase
{
    private readonly ITriajeRepository _repository;
    private readonly TriajeReporteFileService _fileService;

    public DownloadReporteTriajesUseCase(
        ITriajeRepository repository,
        TriajeReporteFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
    }

    public async Task<ReporteTriajesArchivoResponse> ExecuteAsync(
        string formato,
        string? fecha = null,
        string? codigo = null,
        string? nombre = null,
        string? tipoPeso = null,
        decimal? pesoKg = null)
    {
        var (triajes, _) = await _repository.GetAllAsync(
            pagina: 1,
            tamano: int.MaxValue,
            fecha,
            codigo,
            nombre,
            tipoPeso,
            pesoKg);

        var response = triajes.Select(t => new TriajeReporteResponse
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

        return _fileService.Generate(response, formato);
    }
}
