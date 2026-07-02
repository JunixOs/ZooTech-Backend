using ZooTech.Domain.Module_Sanidad.Entities;

namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetHistorialGeneral;

public interface IGetHistorialGeneralInputPort
{
    Task<GetHistorialGeneralOutput> HandleAsync(string? desde, string? hasta, CancellationToken cancellationToken = default);
}

public sealed record GetHistorialGeneralOutput(IEnumerable<TriajeHistorialItem> Items);
