namespace ZooTech.Application.Modules.Module_ProduccionLeche.UseCases.VacasSequia;

public sealed record VacaSequiaActualDto(
    string CodigoVacuno,
    string NombreVacuno,
    DateOnly Desde,
    DateOnly? ProbablementeHasta);

public sealed record VacaSequiaFuturoDto(
    string CodigoVacuno,
    string NombreVacuno,
    DateOnly ProbablementeDesde,
    DateOnly? ProbablementeHasta);

public sealed record VacaSequiaHistoricoDto(
    string CodigoVacuno,
    string NombreVacuno,
    DateOnly Desde,
    DateOnly Hasta);

public sealed record GetVacasSequiaOutput(
    IReadOnlyList<VacaSequiaActualDto> Actuales,
    IReadOnlyList<VacaSequiaFuturoDto> Futuros,
    IReadOnlyList<VacaSequiaHistoricoDto> Historicos);
