namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Rules;

public sealed record VacunoValidationLimits(
    int CodigoMaxLength,
    int InputMaxLength,
    int ObservacionesMaxLength,
    int ObservacionesMaxWords);
