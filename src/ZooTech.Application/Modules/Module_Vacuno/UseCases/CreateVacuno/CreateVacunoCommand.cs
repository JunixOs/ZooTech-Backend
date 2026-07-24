using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

public sealed record CreateVacunoCommand(
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    string TipoAdquisicionCode,
    string RazaCode,
    string ColorCode,
    string SexoCode,
    long? PadreId,
    long? MadreId,
    long GranjaId,
    string? Observaciones,
    decimal? PrecioCompra,
    string? AptoPara) : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Create;

    public string Action => "Create a vacuno";
}