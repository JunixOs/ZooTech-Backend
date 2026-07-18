namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;

public sealed record CreateFecundacionOutput(
    long Id,
    string Codigo,
    DateTime FechaProcedimiento);
