namespace ZooTech.Domain.Ganaderia.Module_Fecundacion.Entities;

public sealed class Fecundacion
{
    public Fecundacion(
        long id,
        string codigo,
        string tipoFecundacionCode,
        long vacunoReceptorId,
        DateTime fechaProcedimiento,
        long responsableId,
        string resultadoCode,
        string? observacionesVeterinarias,
        long? celoRegistroId = null,
        long? actorUsuarioId = null,
        bool machoExterno = false,
        string? machoExternoNombre = null,
        long? vacunoDonanteId = null,
        string? codigoSemen = null,
        string? codigoEmbrion = null)
    {
        Id = id;
        Codigo = codigo;
        TipoFecundacionCode = tipoFecundacionCode;
        VacunoReceptorId = vacunoReceptorId;
        FechaProcedimiento = fechaProcedimiento;
        ResponsableId = responsableId;
        ResultadoCode = resultadoCode;
        ObservacionesVeterinarias = observacionesVeterinarias;
        CeloRegistroId = celoRegistroId;
        ActorUsuarioId = actorUsuarioId;
        MachoExterno = machoExterno;
        MachoExternoNombre = machoExternoNombre;
        VacunoDonanteId = vacunoDonanteId;
        CodigoSemen = codigoSemen;
        CodigoEmbrion = codigoEmbrion;
    }

    public static Fecundacion CreateNew(
        string codigo,
        string tipoFecundacionCode,
        long vacunoReceptorId,
        long? celoRegistroId,
        DateTime fechaProcedimiento,
        long responsableId,
        string resultadoCode,
        string? observacionesVeterinarias,
        long? actorUsuarioId,
        DateTime utcNow,
        bool machoExterno,
        string? machoExternoNombre,
        long? vacunoDonanteId,
        string? codigoSemen = null,
        string? codigoEmbrion = null)
    {
        return new Fecundacion(
            id: 0,
            codigo: codigo,
            tipoFecundacionCode: tipoFecundacionCode,
            vacunoReceptorId: vacunoReceptorId,
            fechaProcedimiento: fechaProcedimiento,
            responsableId: responsableId,
            resultadoCode: resultadoCode,
            observacionesVeterinarias: observacionesVeterinarias,
            celoRegistroId: celoRegistroId,
            actorUsuarioId: actorUsuarioId,
            machoExterno: machoExterno,
            machoExternoNombre: machoExternoNombre,
            vacunoDonanteId: vacunoDonanteId,
            codigoSemen: codigoSemen,
            codigoEmbrion: codigoEmbrion);
    }

    public long Id { get; }
    public string Codigo { get; }
    public string TipoFecundacionCode { get; }
    public long VacunoReceptorId { get; }
    public DateTime FechaProcedimiento { get; }
    public long ResponsableId { get; }
    public string ResultadoCode { get; }
    public string? ObservacionesVeterinarias { get; }
    public long? CeloRegistroId { get; }
    public long? ActorUsuarioId { get; }
    public bool MachoExterno { get; }
    public string? MachoExternoNombre { get; }
    public long? VacunoDonanteId { get; }
    public string? CodigoSemen { get; }
    public string? CodigoEmbrion { get; }
}
