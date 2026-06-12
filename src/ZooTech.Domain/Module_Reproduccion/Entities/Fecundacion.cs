

namespace ZooTech.Domain.Module_Reproduccion.Entities;

public sealed class Fecundacion
{
    public const string TipoMontaNatural = "MONTA_NATURAL";
    public const string TipoInseminacion = "INSEMINACION_ARTIFICIAL";
    public const string TipoTransferencia = "TRANSFERENCIA_EMBRIONES";

    public const string ResultadoPendiente = "PENDIENTE";
    public const string ResultadoExitosa = "EXITOSA";
    public const string ResultadoFallida = "FALLIDA";

    public long Id { get; private set; }
    public string Codigo { get; private set; }
    public string TipoFecundacionCode { get; private set; }
    public long VacunoReceptorId { get; private set; }
    
    // Donante Interno
    public long? VacunoDonanteId { get; private set; }
    
    // Donante Externo
    public long? ExternoDonanteId { get; private set; }
    public string? NombreMachoExterno { get; private set; }

    public DateOnly FechaProcedimiento { get; private set; }
    public long ResponsableId { get; private set; }
    public string ResultadoCode { get; private set; }
    public string? CodigoSemen { get; private set; }
    public string? CodigoEmbrion { get; private set; }
    public string? Observaciones { get; private set; }

    public long? CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public long? UpdatedBy { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Fecundacion(
        long id,
        string codigo,
        string tipoFecundacionCode,
        long vacunoReceptorId,
        long? vacunoDonanteId,
        long? externoDonanteId,
        string? nombreMachoExterno,
        DateOnly fechaProcedimiento,
        long responsableId,
        string resultadoCode,
        string? codigoSemen,
        string? codigoEmbrion,
        string? observaciones,
        long? createdBy,
        DateTime createdAt,
        long? updatedBy,
        DateTime updatedAt)
    {
        Id = id;
        Codigo = codigo;
        TipoFecundacionCode = tipoFecundacionCode;
        VacunoReceptorId = vacunoReceptorId;
        VacunoDonanteId = vacunoDonanteId;
        ExternoDonanteId = externoDonanteId;
        NombreMachoExterno = nombreMachoExterno;
        FechaProcedimiento = fechaProcedimiento;
        ResponsableId = responsableId;
        ResultadoCode = resultadoCode;
        CodigoSemen = codigoSemen;
        CodigoEmbrion = codigoEmbrion;
        Observaciones = observaciones;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }

    public static Fecundacion CreateNew(
        string codigo,
        string tipoFecundacionCode,
        long vacunoReceptorId,
        long? vacunoDonanteId,
        long? externoDonanteId,
        string? nombreMachoExterno,
        DateOnly fechaProcedimiento,
        long responsableId,
        string? codigoSemen,
        string? codigoEmbrion,
        string? observaciones,
        long? createdBy)
    {
        var now = DateTime.UtcNow;
        return new Fecundacion(
            0,
            codigo,
            tipoFecundacionCode,
            vacunoReceptorId,
            vacunoDonanteId,
            externoDonanteId,
            nombreMachoExterno,
            fechaProcedimiento,
            responsableId,
            ResultadoPendiente,
            codigoSemen,
            codigoEmbrion,
            observaciones,
            createdBy,
            now,
            null,
            now
        );
    }

    public void ConfirmResult(string newResult, long? updatedBy)
    {
        if (ResultadoCode != ResultadoPendiente)
        {
            throw new InvalidOperationException($"La fecundación ya fue confirmada con resultado {ResultadoCode}.");
        }

        if (newResult != ResultadoExitosa && newResult != ResultadoFallida)
        {
            throw new ArgumentException("El resultado debe ser EXITOSA o FALLIDA.", nameof(newResult));
        }

        ResultadoCode = newResult;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }
}
