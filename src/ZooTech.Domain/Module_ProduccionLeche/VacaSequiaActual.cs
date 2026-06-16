namespace ZooTech.Domain.Module_ProduccionLeche;

public sealed record VacaSequiaActual(
    string CodigoVacuno,
    string NombreVacuno,
    DateOnly Desde,
    DateOnly? ProbablementeHasta);
