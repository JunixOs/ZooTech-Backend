using ZooTech.Domain.Module_Sanidad.Rules;

namespace ZooTech.Domain.Module_Sanidad.Entities;

public sealed class Triaje
{
    private Triaje(
        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string vacunoNombre,
        string tipoPesoCode,
        decimal pesoKg,
        string? observaciones,
        string estadoRegistroCode,
        long? encargadoUsuarioId,
        long? createdBy,
        long? updatedBy,
        long? deletedBy,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt,
        string? motivoEliminacion)
    {
        Id = id;
        Codigo = codigo;
        FechaHora = fechaHora;
        VacunoId = vacunoId;
        VacunoNombre = vacunoNombre;
        TipoPesoCode = tipoPesoCode;
        PesoKg = pesoKg;
        Observaciones = observaciones;
        EstadoRegistroCode = estadoRegistroCode;
        EncargadoUsuarioId = encargadoUsuarioId;
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
        DeletedBy = deletedBy;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
        MotivoEliminacion = motivoEliminacion;
    }

    public long Id { get; private set; }
    public string Codigo { get; private set; }
    public DateTime FechaHora { get; private set; }
    public long VacunoId { get; private set; }
    public string VacunoNombre { get; private set; }
    public string TipoPesoCode { get; private set; }
    public decimal PesoKg { get; private set; }
    public string? Observaciones { get; private set; }
    public string EstadoRegistroCode { get; private set; }
    public long? EncargadoUsuarioId { get; private set; }
    public long? CreatedBy { get; private set; }
    public long? UpdatedBy { get; private set; }
    public long? DeletedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? MotivoEliminacion { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;

    public static Triaje CreateNew(
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string tipoPesoCode,
        decimal pesoKg,
        string? observaciones,
        long? encargadoUsuarioId,
        DateTime utcNow)
    {
        Validate(vacunoId, tipoPesoCode, pesoKg, fechaHora);
        return new Triaje(
            id: 0,
            codigo: codigo.Trim(),
            fechaHora: fechaHora,
            vacunoId: vacunoId,
            vacunoNombre: string.Empty,
            tipoPesoCode: tipoPesoCode.Trim(),
            pesoKg: pesoKg,
            observaciones: SanitizeObservaciones(observaciones),
            estadoRegistroCode: "ACTIVO",
            encargadoUsuarioId: encargadoUsuarioId,
            createdBy: encargadoUsuarioId,
            updatedBy: encargadoUsuarioId,
            deletedBy: null,
            createdAt: utcNow,
            updatedAt: utcNow,
            deletedAt: null,
            motivoEliminacion: null);
    }

    public static Triaje Rehydrate(
        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string vacunoNombre,
        string tipoPesoCode,
        decimal pesoKg,
        string? observaciones,
        string estadoRegistroCode,
        long? encargadoUsuarioId,
        long? createdBy,
        long? updatedBy,
        long? deletedBy,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt,
        string? motivoEliminacion)
    {
        Validate(vacunoId, tipoPesoCode, pesoKg, fechaHora);

        return new Triaje(
            id, 
            codigo, 
            fechaHora, 
            vacunoId, 
            vacunoNombre,
            tipoPesoCode, 
            pesoKg, 
            SanitizeObservaciones(observaciones), 
            estadoRegistroCode,
            encargadoUsuarioId, 
            createdBy, 
            updatedBy, 
            deletedBy,
            createdAt, 
            updatedAt, 
            deletedAt, 
            motivoEliminacion);
    }

    public void Update(
        string tipoPesoCode,
        decimal pesoKg,
        string? observaciones,
        long? encargadoUsuarioId,
        DateTime utcNow)
    {
        if (IsDeleted)
            throw new InvalidOperationException("No se puede actualizar un triaje eliminado.");
        TriajeRule.ValidarTipoPesoCode(tipoPesoCode);
        TriajeRule.ValidarPesoKg(pesoKg);
        TipoPesoCode = tipoPesoCode.Trim();
        PesoKg = pesoKg;
        Observaciones = SanitizeObservaciones(observaciones);
        EncargadoUsuarioId = encargadoUsuarioId;
        UpdatedBy = encargadoUsuarioId;
        UpdatedAt = utcNow;
    }

    public void SoftDelete(string motivoEliminacion, long? actorId, DateTime utcNow)
    {
        if (IsDeleted)
            throw new InvalidOperationException("El triaje ya se encuentra eliminado.");
        TriajeRule.ValidarMotivoEliminacion(motivoEliminacion);
        MotivoEliminacion = motivoEliminacion;
        DeletedAt = utcNow;
        DeletedBy = actorId;
        UpdatedAt = utcNow;
        UpdatedBy = actorId;
    }

    private static string? SanitizeObservaciones(string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(observaciones))
            return null;

        var value = observaciones.Trim();
        return value.Length > 150 ? value[..150] : value;
    }

    private static void Validate(
        long vacunoId,
        string tipoPesoCode,
        decimal pesoKg,
        DateTime fechaHora
        )
    {
        TriajeRule.ValidarFechaHora(fechaHora);
        TriajeRule.ValidarFechaHoraFutura(fechaHora);

        TriajeRule.ValidarTipoPesoCode(tipoPesoCode);
        TriajeRule.ValidarPesoKg(pesoKg);
        TriajeRule.ValidarVacunoId(vacunoId);
    }
}
