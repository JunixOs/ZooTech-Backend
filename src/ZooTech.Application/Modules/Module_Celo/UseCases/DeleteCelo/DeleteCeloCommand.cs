namespace ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo;

public sealed record DeleteCeloCommand(
    long Id,
    string MotivoEliminacion);
