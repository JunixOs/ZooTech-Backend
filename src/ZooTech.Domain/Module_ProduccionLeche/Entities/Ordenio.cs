using ZooTech.Domain.Module_ProduccionLeche.Rules;

namespace ZooTech.Domain.Module_ProduccionLeche.Entities;

public sealed class Ordenio
{
    private Ordenio(
        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string nombreVacuno,
        long encargadoUsuarioId,
        decimal litros,
        string estadoOrdenioCode,
        string? observaciones,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt,
        long? createdBy,
        long? updatedBy,
        long? deletedBy)
    {
        Id = id;
        Codigo = codigo;
        FechaHora = fechaHora;
        VacunoId = vacunoId;
        NombreVacuno = nombreVacuno;
        EncargadoUsuarioId = encargadoUsuarioId;
        Litros = litros;
        EstadoOrdenioCode = estadoOrdenioCode;
        Observaciones = observaciones;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
        DeletedBy = deletedBy;
    }

    public long Id { get; }
    public string Codigo { get; private set; }
    public DateTime FechaHora { get; private set; }
    public long VacunoId { get; private set; }
    public string NombreVacuno { get; private set; }
    public long EncargadoUsuarioId { get; private set; }
    public decimal Litros { get; private set; }
    public string EstadoOrdenioCode { get; private set; }
    public string? Observaciones { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? MotivoEliminacion { get; private set; }
    public long? CreatedBy { get; }
    public long? UpdatedBy { get; private set; }
    public long? DeletedBy { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;

    public static Ordenio CreateNew(
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        long encargadoUsuarioId,
        decimal litros,
        string estadoOrdenioCode,
        string? observaciones,
        long? actorUsuarioId,
        DateTime utcNow)
    {
        Validate(codigo, fechaHora, vacunoId, encargadoUsuarioId, litros, estadoOrdenioCode);

        return new Ordenio(
            id: 0,
            codigo: codigo.Trim(),
            fechaHora: fechaHora,
            vacunoId: vacunoId,
            nombreVacuno: string.Empty,
            encargadoUsuarioId: encargadoUsuarioId,
            litros: litros,
            estadoOrdenioCode: estadoOrdenioCode.Trim(),
            observaciones: SanitizeObservaciones(observaciones),
            createdAt: utcNow,
            updatedAt: utcNow,
            deletedAt: null,
            createdBy: actorUsuarioId,
            updatedBy: actorUsuarioId,
            deletedBy: null);
    }

    public static Ordenio Rehydrate(
        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string nombreVacuno,
        long encargadoUsuarioId,
        decimal litros,
        string estadoOrdenioCode,
        string? observaciones,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt,
        string? motivoEliminacion,
        long? createdBy,
        long? updatedBy,
        long? deletedBy)
    {
        Validate(codigo, fechaHora, vacunoId, encargadoUsuarioId, litros, estadoOrdenioCode);

        return new Ordenio(
            id,
            codigo.Trim(),
            fechaHora,
            vacunoId,
            nombreVacuno,
            encargadoUsuarioId,
            litros,
            estadoOrdenioCode.Trim(),
            SanitizeObservaciones(observaciones),
            createdAt,
            updatedAt,
            deletedAt,
            createdBy,
            updatedBy,
            deletedBy);
    }

    public void Update(
        DateTime fechaHora,
        long encargadoUsuarioId,
        decimal litros,
        string estadoOrdenioCode,
        string? observaciones,
        long? actorUsuarioId,
        DateTime utcNow)
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("No se puede actualizar un ordeño eliminado.");
        }

        Validate(Codigo, fechaHora, VacunoId, encargadoUsuarioId, litros, estadoOrdenioCode);

        FechaHora = fechaHora;
        EncargadoUsuarioId = encargadoUsuarioId;
        Litros = litros;
        EstadoOrdenioCode = estadoOrdenioCode.Trim();
        Observaciones = SanitizeObservaciones(observaciones);
        UpdatedBy = actorUsuarioId;
        UpdatedAt = utcNow;
    }

    public void SoftDelete(string motivoEliminacion, long? actorUsuarioId, DateTime utcNow)
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("El ordeño ya se encuentra eliminado.");
        }

        if (string.IsNullOrWhiteSpace(motivoEliminacion))
        {
            throw new ArgumentException("El motivo de eliminación es obligatorio.");
        }

        DeletedAt = utcNow;
        DeletedBy = actorUsuarioId;
        MotivoEliminacion = motivoEliminacion.Trim();
        UpdatedAt = utcNow;
        UpdatedBy = actorUsuarioId;
    }

    private static void Validate(
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        long encargadoUsuarioId,
        decimal litros,
        string estadoOrdenioCode,
        string? observaciones = null
        )
    {
        OrdenioRule.ValidarCodigo(codigo);
        OrdenioRule.ValidarFechaHora(fechaHora);
        OrdenioRule.ValidarLitros(litros);
        OrdenioRule.ValidarEstadoOrdenioCode(estadoOrdenioCode);
        OrdenioRule.ValidarVacunoId(vacunoId);
        OrdenioRule.ValidarEncargadoUsuarioId(encargadoUsuarioId);
        OrdenioRule.ValidarObservaciones(observaciones); // Solo valida longitud si no es null, el método de validación se encarga de eso

    }

    private static string? SanitizeObservaciones(string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(observaciones))
        {
            return null;
        }

        var value = observaciones.Trim();
        return value.Length > 150 ? value[..150] : value;
    }
}
