namespace ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.UpdateFecundacionEstado;

public sealed record UpdateFecundacionEstadoCommand(
    long FecundacionId,
    string? EstadoFecundacion,
    long? UpdatedBy);
