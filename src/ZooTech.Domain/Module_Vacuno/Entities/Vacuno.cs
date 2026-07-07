using ZooTech.Domain.Module_Vacuno.Rules;

namespace ZooTech.Domain.Module_Vacuno.Entities;

public sealed class Vacuno
{
    public long Id { get; }
    public string Codigo { get; private set; }
    public string Nombre { get; private set; }
    public DateOnly FechaNacimiento { get; private set; }
    public string TipoAdquisicionCode { get; private set; }
    public string RazaCode { get; private set; }
    public string ColorCode { get; private set; }
    public string SexoCode { get; private set; }
    public long? PadreId { get; private set; }
    public long? MadreId { get; private set; }
    public long GranjaId { get; private set; }
    public string? Observaciones { get; private set; }
    public DateOnly FechaRegistro { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? MotivoEliminacion { get; private set; }
    public long? CreatedBy { get; }
    public long? UpdatedBy { get; private set; }
    public long? DeletedBy { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;

    private Vacuno(
        long id,
        string codigo,
        string nombre,
        DateOnly fechaNacimiento,
        string tipoAdquisicionCode,
        string razaCode,
        string colorCode,
        string sexoCode,
        long? padreId,
        long? madreId,
        long granjaId,
        string? observaciones,
        DateOnly fechaRegistro,
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
        Nombre = nombre;
        FechaNacimiento = fechaNacimiento;
        TipoAdquisicionCode = tipoAdquisicionCode;
        RazaCode = razaCode;
        ColorCode = colorCode;
        SexoCode = sexoCode;
        PadreId = padreId;
        MadreId = madreId;
        GranjaId = granjaId;
        Observaciones = observaciones;
        FechaRegistro = fechaRegistro;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        DeletedAt = deletedAt;
        MotivoEliminacion = motivoEliminacion;
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
        DeletedBy = deletedBy;
    }


    public static Vacuno CreateNew(
        string codigo,
        string nombre,
        DateOnly fechaNacimiento,
        string tipoAdquisicionCode,
        string razaCode,
        string colorCode,
        string sexoCode,
        long? padreId,
        long? madreId,
        long granjaId,
        string? observaciones,
        long? actorUsuarioId,
        DateTime utcNow)
    {
        Validate(codigo, nombre, razaCode, sexoCode, tipoAdquisicionCode, colorCode, granjaId, observaciones);

        return new Vacuno(
            id: 0,
            codigo: codigo.Trim(),
            nombre: nombre.Trim(),
            fechaNacimiento: fechaNacimiento,
            tipoAdquisicionCode: tipoAdquisicionCode.Trim(),
            razaCode: razaCode.Trim(),
            colorCode: colorCode.Trim(),
            sexoCode: sexoCode.Trim(),
            padreId: padreId,
            madreId: madreId,
            granjaId: granjaId,
            observaciones: SanitizeObservaciones(observaciones),
            fechaRegistro: DateOnly.FromDateTime(utcNow),
            createdAt: utcNow,
            updatedAt: utcNow,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: actorUsuarioId,
            updatedBy: actorUsuarioId,
            deletedBy: null);
    }

    public static Vacuno Rehydrate(
        long id,
        string codigo,
        string nombre,
        DateOnly fechaNacimiento,
        string tipoAdquisicionCode,
        string razaCode,
        string colorCode,
        string sexoCode,
        long? padreId,
        long? madreId,
        long granjaId,
        string? observaciones,
        DateOnly fechaRegistro,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt,
        string? motivoEliminacion,
        long? createdBy,
        long? updatedBy,
        long? deletedBy)
    {
        ValidateRehydrate(
            id,
            codigo,
            nombre,
            fechaNacimiento,
            tipoAdquisicionCode,
            razaCode,
            colorCode,
            sexoCode,
            granjaId,
            fechaRegistro,
            createdAt,
            updatedAt,
            deletedAt);

        return new Vacuno(
            id,
            codigo.Trim(),
            nombre.Trim(),
            fechaNacimiento,
            tipoAdquisicionCode.Trim(),
            razaCode.Trim(),
            colorCode.Trim(),
            sexoCode.Trim(),
            padreId,
            madreId,
            granjaId,
            observaciones,
            fechaRegistro,
            createdAt,
            updatedAt,
            deletedAt,
            motivoEliminacion,
            createdBy,
            updatedBy,
            deletedBy);
    }

    public void Update(
        string nombre,
        DateOnly fechaNacimiento,
        string tipoAdquisicionCode,
        string razaCode,
        string colorCode,
        string sexoCode,
        long? padreId,
        long? madreId,
        long granjaId,
        string? observaciones,
        long? actorUsuarioId,
        DateTime utcNow)
    {
        if (IsDeleted)
            throw new InvalidOperationException("No se puede actualizar un vacuno eliminado.");

        Validate(Codigo, nombre, razaCode, sexoCode, tipoAdquisicionCode, colorCode, granjaId, observaciones);

        Nombre = nombre.Trim();
        FechaNacimiento = fechaNacimiento;
        TipoAdquisicionCode = tipoAdquisicionCode.Trim();
        RazaCode = razaCode.Trim();
        ColorCode = colorCode.Trim();
        SexoCode = sexoCode.Trim();
        PadreId = padreId;
        MadreId = madreId;
        GranjaId = granjaId;
        Observaciones = SanitizeObservaciones(observaciones);
        UpdatedBy = actorUsuarioId;
        UpdatedAt = utcNow;
    }

    public void SoftDelete(string motivoEliminacion, long? actorUsuarioId, DateTime utcNow)
    {
        if (IsDeleted)
            throw new InvalidOperationException("El vacuno ya se encuentra eliminado.");

        VacunoRule.ValidarMotivoEliminacion(motivoEliminacion);

        DeletedAt = utcNow;
        DeletedBy = actorUsuarioId;
        MotivoEliminacion = motivoEliminacion.Trim();
        UpdatedAt = utcNow;
        UpdatedBy = actorUsuarioId;
    }

    private static void Validate(
        string codigo,
        string nombre,
        string razaCode,
        string sexoCode,
        string tipoAdquisicionCode,
        string colorCode,
        long granjaId,
        string? observaciones = null)
    {
        VacunoRule.ValidarCodigo(codigo);
        VacunoRule.ValidarNombre(nombre);
        VacunoRule.ValidarRazaCode(razaCode);
        VacunoRule.ValidarSexoCode(sexoCode);
        VacunoRule.ValidarTipoAdquisicionCode(tipoAdquisicionCode);
        VacunoRule.ValidarColorCode(colorCode);
        VacunoRule.ValidarGranjaId(granjaId);
        VacunoRule.ValidarObservaciones(observaciones);
    }

    private static void ValidateRehydrate(
        long id,
        string codigo,
        string nombre,
        DateOnly fechaNacimiento,
        string tipoAdquisicionCode,
        string razaCode,
        string colorCode,
        string sexoCode,
        long granjaId,
        DateOnly fechaRegistro,
        DateTime createdAt,
        DateTime updatedAt,
        DateTime? deletedAt)
    {
        VacunoRule.ValidarIdPersistido(id);
        VacunoRule.ValidarCampoPersistido(codigo, nameof(Codigo));
        VacunoRule.ValidarCampoPersistido(nombre, nameof(Nombre));
        VacunoRule.ValidarCampoPersistido(tipoAdquisicionCode, nameof(TipoAdquisicionCode));
        VacunoRule.ValidarCampoPersistido(razaCode, nameof(RazaCode));
        VacunoRule.ValidarCampoPersistido(colorCode, nameof(ColorCode));
        VacunoRule.ValidarCampoPersistido(sexoCode, nameof(SexoCode));
        VacunoRule.ValidarGranjaId(granjaId);
        VacunoRule.ValidarFechasPersistidas(fechaNacimiento, fechaRegistro, createdAt, updatedAt, deletedAt);
    }

    private static string? SanitizeObservaciones(string? observaciones)
    {
        if (string.IsNullOrWhiteSpace(observaciones))
            return null;

        var value = observaciones.Trim();
        return value.Length > 150 ? value[..150] : value;
    }
}
