namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;

public sealed record VacaSequiaActualResponse(
    string CodigoVacuno,
    string NombreVacuno,
    DateOnly Desde,
    DateOnly? ProbablementeHasta);

public sealed record VacaSequiaFuturoResponse(
    string CodigoVacuno,
    string NombreVacuno,
    DateOnly ProbablementeDesde,
    DateOnly? ProbablementeHasta);

public sealed record VacaSequiaHistoricoResponse(
    string CodigoVacuno,
    string NombreVacuno,
    DateOnly Desde,
    DateOnly Hasta);

public sealed record VacasSequiaResponse(
    List<VacaSequiaActualResponse> Actuales,
    List<VacaSequiaFuturoResponse> Futuros,
    List<VacaSequiaHistoricoResponse> Historicos);
