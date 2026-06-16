namespace ZooTech.Domain.Module_ProduccionLeche;

public sealed record VacaSequiaHistorico(
    string CodigoVacuno,
    string NombreVacuno,
    DateOnly Desde,
    DateOnly Hasta);
