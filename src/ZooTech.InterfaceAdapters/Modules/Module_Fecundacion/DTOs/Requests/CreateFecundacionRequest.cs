using System.ComponentModel.DataAnnotations;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Requests;

public sealed record CreateFecundacionRequest(
    [Required] string TipoFecundacionCode,
    [Required] long VacunoReceptorId,
    long? CeloRegistroId,
    [Required] DateTime FechaProcedimiento,
    [Required] string ResponsableName,
    [Required] string ResultadoCode,
    string? Observaciones,
    bool MachoExterno,
    string? MachoExternoNombre,
    long? VacunoDonanteId);
