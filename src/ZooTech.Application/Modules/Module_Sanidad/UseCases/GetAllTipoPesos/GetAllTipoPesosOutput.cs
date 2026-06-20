namespace ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllTipoPesos;

public sealed record GetAllTipoPesosOutput(IReadOnlyList<TipoPesoItemOutput> Items);

public sealed record TipoPesoItemOutput(string Code, string Nombre);
