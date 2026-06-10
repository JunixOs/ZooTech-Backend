namespace ZooTech.Application.Modules.Module_Celo.UseCases.EliminarCelo;

public sealed record EliminarCeloCommand(
    long Id,
    string MotivoEliminacion);
