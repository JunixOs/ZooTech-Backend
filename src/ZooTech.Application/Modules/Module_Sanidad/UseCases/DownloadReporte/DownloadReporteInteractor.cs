namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.DownloadReporte;

using ZooTech.Application.Modules.Module_Sanidad.DTOs.Responses;
using ZooTech.Application.Modules.Module_Sanidad.Services;
using ZooTech.Domain.Module_Sanidad.Interfaces;

public sealed class DownloadReporteInteractor : IDownloadReporteInputPort
{
    private readonly ITriajeRepository _repository;
    private readonly TriajeReporteFileService _fileService;

    public DownloadReporteInteractor(
        ITriajeRepository repository,
        TriajeReporteFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
    }

    public async Task<DownloadReporteOutput> HandleAsync(DownloadReporteCommand command, CancellationToken cancellationToken = default)
    {
        var (triajes, _) = await _repository.GetAllAsync(
            pagina: 1,
            tamano: int.MaxValue,
            command.Fecha,
            command.Codigo,
            command.Nombre,
            command.TipoPeso,
            command.PesoKg,
            cancellationToken);

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
        return new DownloadReporteOutput(fileResult.Content, fileResult.ContentType, fileResult.FileName, fileResult.Message);
    }
}