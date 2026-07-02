namespace ZooTech.Domain.Module_Fecundacion.Entities;

public sealed class Fecundacion
{
    private Fecundacion(
        long id,
        string codigo,
        string tipoFecundacionCode,
        long vacunoReceptorId,
        long? celoRegistroId,
        DateTime fechaProcedimiento,
        long responsableId,
        string resultadoCode,
        string? observacionesVeterinarias,
        long? createdBy,
        long? updatedBy,
        DateTime createdAt,
        DateTime updatedAt,
        bool machoExterno,
        string? machoExternoNombre,
        long? vacunoDonanteId)
    {
        Id = id;
        Codigo = codigo;
        TipoFecundacionCode = tipoFecundacionCode;
        VacunoReceptorId = vacunoReceptorId;
        CeloRegistroId = celoRegistroId;
        FechaProcedimiento = fechaProcedimiento;
        ResponsableId = responsableId;
        ResultadoCode = resultadoCode;
        ObservacionesVeterinarias = observacionesVeterinarias;
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        MachoExterno = machoExterno;
        MachoExternoNombre = machoExternoNombre;
        VacunoDonanteId = vacunoDonanteId;
    }

    public long Id { get; }
    public string Codigo { get; private set; }
    public string TipoFecundacionCode { get; private set; }
    public long VacunoReceptorId { get; private set; }
    public long? CeloRegistroId { get; private set; }
    public DateTime FechaProcedimiento { get; private set; }
    public long ResponsableId { get; private set; }
    public string ResultadoCode { get; private set; }
    public string? ObservacionesVeterinarias { get; private set; }
    public long? CreatedBy { get; }
    public long? UpdatedBy { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    // Campos temporales para transporte de datos de Donante
    public bool MachoExterno { get; }
    public string? MachoExternoNombre { get; }
    public long? VacunoDonanteId { get; }

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
        long? vacunoDonanteId)
    {
        // Validaciones de negocio básicas
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código de fecundación no puede estar vacío.", nameof(codigo));

        if (string.IsNullOrWhiteSpace(tipoFecundacionCode))
            throw new ArgumentException("El tipo de fecundación es obligatorio.", nameof(tipoFecundacionCode));

        if (vacunoReceptorId <= 0)
            throw new ArgumentException("El vacuno receptor es obligatorio.", nameof(vacunoReceptorId));

        if (responsableId <= 0)
            throw new ArgumentException("El responsable es obligatorio.", nameof(responsableId));

        if (string.IsNullOrWhiteSpace(resultadoCode))
            throw new ArgumentException("El resultado de la fecundación es obligatorio.", nameof(resultadoCode));

        if (!machoExterno && (!vacunoDonanteId.HasValue || vacunoDonanteId <= 0))
            throw new ArgumentException("Debe seleccionar un vacuno donante interno si no es macho externo.", nameof(vacunoDonanteId));

        if (machoExterno && string.IsNullOrWhiteSpace(machoExternoNombre))
            throw new ArgumentException("El nombre del macho externo es obligatorio si se indica que es macho externo.", nameof(machoExternoNombre));

        return new Fecundacion(
            id: 0,
            codigo: codigo.Trim(),
            tipoFecundacionCode: tipoFecundacionCode.Trim(),
            vacunoReceptorId: vacunoReceptorId,
            celoRegistroId: celoRegistroId,
            fechaProcedimiento: fechaProcedimiento,
            responsableId: responsableId,
            resultadoCode: resultadoCode.Trim(),
            observacionesVeterinarias: observacionesVeterinarias?.Trim(),
            createdBy: actorUsuarioId,
            updatedBy: actorUsuarioId,
            createdAt: utcNow,
            updatedAt: utcNow,
            machoExterno: machoExterno,
            machoExternoNombre: machoExternoNombre?.Trim(),
            vacunoDonanteId: vacunoDonanteId);
    }
}
