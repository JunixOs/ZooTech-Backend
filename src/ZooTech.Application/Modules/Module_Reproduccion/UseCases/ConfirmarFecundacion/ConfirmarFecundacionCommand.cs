namespace ZooTech.Application.Modules.Module_Reproduccion.UseCases.ConfirmarFecundacion;

public sealed record ConfirmarFecundacionCommand(
    long FecundacionId,
    string NuevoResultadoCode,
    long? CurrentUserId
);
