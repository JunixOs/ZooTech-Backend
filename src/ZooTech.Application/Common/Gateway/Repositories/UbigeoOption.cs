namespace ZooTech.Application.Common.Gateway.Repositories;

public sealed record UbigeoOption(
    string Codigo,
    string Nombre,
    IReadOnlyList<UbigeoOption> Hijos);
