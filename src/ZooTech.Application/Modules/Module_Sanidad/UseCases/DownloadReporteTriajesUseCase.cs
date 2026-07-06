using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Application.Modules.Module_Sanidad.Services;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases;

public interface IDownloadReporteTriajesInputPort
{
    Task<DownloadReporteTriajesOutput> HandleAsync(DownloadReporteTriajesCommand command, CancellationToken cancellationToken = default);
}

public sealed record DownloadReporteTriajesCommand(
    string Formato,
    string? Fecha = null,
    string? Codigo = null,
    string? Nombre = null,
    string? TipoPeso = null,
    decimal? PesoKg = null);

public sealed record DownloadReporteTriajesOutput(
    byte[] Content,
    string ContentType,
    string FileName,
    string Message);

public sealed class DownloadReporteTriajesInteractor : IDownloadReporteTriajesInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly TriajeReporteFileService _fileService;

    public DownloadReporteTriajesInteractor(
        ITriajeRepository repository,
        TriajeReporteFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
    }

    public async Task<DownloadReporteTriajesOutput> HandleAsync(DownloadReporteTriajesCommand command, CancellationToken cancellationToken = default)
    {
        var (triajes, _) = await _repository.GetAllAsync(
     pagina: 1,
     tamano: int.MaxValue,
     fecha: command.Fecha,
     codigo: command.Codigo,
     nombre: command.Nombre,
     tipoPeso: command.TipoPeso,
     pesoKg: command.PesoKg,
     cancellationToken: cancellationToken);

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

        var fileResult = _fileService.Generate(response, command.Formato);
        return new DownloadReporteTriajesOutput(fileResult.Content, fileResult.ContentType, fileResult.FileName, fileResult.Message);
    }
}
