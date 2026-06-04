namespace ZooTech.API.Configuration;

public sealed class VacunoRequirementOptions
{
    public const string SectionName = "VacunoRequirements";

    public VerVacunoRequirementOptions VerVacuno { get; init; } = new();

    public GraficoVacunosActividadRequirementOptions GraficoVacunosEnActividad { get; init; } = new();
}

public sealed class VerVacunoRequirementOptions
{
    public bool ExcluirEliminados { get; init; } = true;

    public bool PermitirBusquedaCodigoSinSeparadores { get; init; } = true;
}

public sealed class GraficoVacunosActividadRequirementOptions
{
    public int DiasPorDefecto { get; init; } = 30;

    public int MaximoDiasRango { get; init; } = 366;

    public bool ContarEliminadosHastaFechaEliminacion { get; init; } = true;
}
