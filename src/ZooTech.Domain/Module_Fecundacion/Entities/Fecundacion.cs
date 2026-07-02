namespace ZooTech.Domain.Module_Fecundacion.Entities;

public sealed class Fecundacion
{
    public Fecundacion(
        long id,
        string codigo,
        string tipoFecundacionCode,
        long vacunoReceptorId,
        DateOnly fechaProcedimiento,
        long responsableId,
        string resultadoCode,
        string? observacionesVeterinarias)
    {
        Id = id;
        Codigo = codigo;
        TipoFecundacionCode = tipoFecundacionCode;
        VacunoReceptorId = vacunoReceptorId;
        FechaProcedimiento = fechaProcedimiento;
        ResponsableId = responsableId;
        ResultadoCode = resultadoCode;
        ObservacionesVeterinarias = observacionesVeterinarias;
    }

    public long Id { get; }
    public string Codigo { get; }
    public string TipoFecundacionCode { get; }
    public long VacunoReceptorId { get; }
    public DateOnly FechaProcedimiento { get; }
    public long ResponsableId { get; }
    public string ResultadoCode { get; }
    public string? ObservacionesVeterinarias { get; }
}
