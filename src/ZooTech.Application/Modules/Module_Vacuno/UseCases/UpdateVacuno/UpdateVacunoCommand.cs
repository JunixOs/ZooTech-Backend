using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Modules.Module_Vacuno.Services;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

public sealed record UpdateVacunoCommand(
    long Id,
    string Nombre,
    DateOnly FechaNacimiento,
    string TipoAdquisicionCode,
    string RazaCode,
    string ColorCode,
    string SexoCode,
    string? CodigoPadre,
    string? CodigoMadre,
    long? GranjaId,
    string? Granja,
    string? CodigoDistrito,
    string? Observaciones,
    decimal? PrecioCompra,
    string? AptoPara,
    DateOnly? FechaEspecificacion = null,
    VacunoPhotoUpload? Foto = null) : IAuditableRequest
{
    public AuditEventType EventType => AuditEventType.Update;

    public string Action => "Update vacuno";
}
