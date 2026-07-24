namespace ZooTech.Application.Modules.Module_Vacuno.Services;

public interface IVacunoReferenceResolver
{
    Task<VacunoReferenceResolution> ResolveAsync(
        VacunoReferenceData referenceData,
        CancellationToken cancellationToken = default);
}

public sealed record VacunoReferenceData(
    string OwnCodigo,
    string? CodigoPadre,
    string? CodigoMadre,
    long? GranjaId,
    string? GranjaNombre,
    string? CodigoDistrito);

public sealed record VacunoReferenceResolution(
    long? PadreId,
    long? MadreId,
    long? GranjaId,
    VacunoGranjaCreation? GranjaToCreate,
    VacunoReferenceError? Error)
{
    public bool Success => Error is null;

    public static VacunoReferenceResolution Ok(long? padreId, long? madreId, long granjaId)
        => new(padreId, madreId, granjaId, null, null);

    public static VacunoReferenceResolution OkNewGranja(
        long? padreId,
        long? madreId,
        string nombre,
        string codigoDistrito)
        => new(padreId, madreId, null, new VacunoGranjaCreation(nombre, codigoDistrito), null);

    public static VacunoReferenceResolution Fail(VacunoReferenceError error)
        => new(null, null, null, null, error);
}

public sealed record VacunoGranjaCreation(string Nombre, string CodigoDistrito);

public sealed record VacunoReferenceError(
    string? Field,
    string Message,
    VacunoReferenceErrorKind Kind);

public enum VacunoReferenceErrorKind
{
    Validation,
    NotFound
}
