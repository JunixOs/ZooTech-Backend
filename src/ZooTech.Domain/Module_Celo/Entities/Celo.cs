using ZooTech.Domain.Module_Celo.Rules;

namespace ZooTech.Domain.Module_Celo.Entities;

public sealed class Celo
{
    private Celo(
        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string vacunoCodigo,
        string nombreVacuno,
        long encargadoUsuarioId,
        string? observaciones,
        string estadoRegistroCode,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt,
        string? motivoEliminacion,
        long? createdBy,
        long? updatedBy,
        long? deletedBy)
    {
        Id = id;
        Codigo = codigo;
        FechaHora = fechaHora;
        VacunoId = vacunoId;
        VacunoCodigo = vacunoCodigo;
        NombreVacuno = nombreVacuno;
        EncargadoUsuarioId = encargadoUsuarioId;
        Observaciones = observaciones;
        EstadoRegistroCode = estadoRegistroCode;
        CaracteristicaCodes = new List<string>();
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
        MotivoEliminacion = motivoEliminacion;
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
        DeletedBy = deletedBy;
    }

    public long Id { get; }
    public string Codigo { get; private set; }
    public DateTime FechaHora { get; private set; }
    public long VacunoId { get; private set; }
    public string VacunoCodigo { get; private set; }
    public string NombreVacuno { get; private set; }
    public long EncargadoUsuarioId { get; private set; }
    public string? Observaciones { get; private set; }
    public string EstadoRegistroCode { get; private set; }
    public IList<string> CaracteristicaCodes { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? MotivoEliminacion { get; private set; }
    public long? CreatedBy { get; }
    public long? UpdatedBy { get; private set; }
    public long? DeletedBy { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;

    public static Celo CreateNew(
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        long encargadoUsuarioId,
        string? observaciones,
        string estadoRegistroCode,
        List<string>? caracteristicaCodes,
        long? actorUsuarioId,
        DateTime utcNow)
    {
        Validate(codigo, fechaHora, vacunoId, encargadoUsuarioId, estadoRegistroCode, observaciones);

        var celo = new Celo(
            id: 0,
            codigo: codigo.Trim(),
            fechaHora: fechaHora,
            vacunoId: vacunoId,
            vacunoCodigo: string.Empty,
            nombreVacuno: string.Empty,
            encargadoUsuarioId: encargadoUsuarioId,
            observaciones: SanitizeObservaciones(observaciones),
            estadoRegistroCode: estadoRegistroCode.Trim(),
            createdAt: utcNow,
            updatedAt: utcNow,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: actorUsuarioId,
            updatedBy: actorUsuarioId,
            deletedBy: null);

        if (caracteristicaCodes is { Count: > 0 })
        {
            foreach (var code in caracteristicaCodes)
            {
                celo.CaracteristicaCodes.Add(code);
            }
        }

        return celo;
    }

    public static Celo Rehydrate(
        long id,
        string codigo,
        DateTime fechaHora,
        long vacunoId,
        string vacunoCodigo,
        string nombreVacuno,
        long encargadoUsuarioId,
        string? observaciones,
        string estadoRegistroCode,
        List<string> caracteristicaCodes,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt,
        string? motivoEliminacion,
        long? createdBy,
        long? updatedBy,
        long? deletedBy)
    {
        Validate(codigo, fechaHora, vacunoId, encargadoUsuarioId, estadoRegistroCode, observaciones);

        var celo = new Celo(
            id,
            codigo.Trim(),
            fechaHora,
            vacunoId,
            vacunoCodigo,
            nombreVacuno,
            encargadoUsuarioId,
            SanitizeObservaciones(observaciones),
            estadoRegistroCode.Trim(),
            createdAt,
            updatedAt,
            deletedAt,
            motivoEliminacion,
            createdBy,
            updatedBy,
            deletedBy);

        foreach (var code in caracteristicaCodes)
        {
            celo.CaracteristicaCodes.Add(code);
        }

        return celo;
    }

    public void Update(
        string? observaciones,
        List<string>? caracteristicaCodes,
        long? actorUsuarioId,
        DateTime utcNow)
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("No se puede actualizar un registro de celo eliminado.");
        }

        Validate(Codigo, FechaHora, VacunoId, EncargadoUsuarioId, EstadoRegistroCode, observaciones);

        Observaciones = SanitizeObservaciones(observaciones);

        CaracteristicaCodes.Clear();
        if (caracteristicaCodes is { Count: > 0 })
        {
            foreach (var code in caracteristicaCodes)
            {
                CaracteristicaCodes.Add(code);
            }
        }

        UpdatedBy = actorUsuarioId;
        UpdatedAt = utcNow;
    }

    public void SoftDelete(string motivoEliminacion, long? actorUsuarioId, DateTime utcNow)
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("El registro de celo ya se encuentra eliminado.");
        }

        CeloRule.ValidarMotivoEliminacion(motivoEliminacion);

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
        string estadoRegistroCode,
        string? observaciones = null)
    {
        CeloRule.ValidarCodigo(codigo);
        CeloRule.ValidarFechaHora(fechaHora);
        CeloRule.ValidarVacunoId(vacunoId);
        CeloRule.ValidarEncargadoUsuarioId(encargadoUsuarioId);
        CeloRule.ValidarEstadoRegistroCode(estadoRegistroCode);
        CeloRule.ValidarObservaciones(observaciones);
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
