namespace ZooTech.Domain.Module_ProduccionLeche;

public sealed record VacaSequiaFuturo(
    string CodigoVacuno,
    string NombreVacuno,
    DateOnly ProbablementeDesde,
    DateOnly? ProbablementeHasta);
