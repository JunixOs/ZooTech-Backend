namespace ZooTech.Application.Modules.Module_Reproduccion.UseCases.ConfirmarFecundacion;

public sealed record ConfirmarFecundacionOutput(
    long FecundacionId,
    string NuevoResultadoCode
);
