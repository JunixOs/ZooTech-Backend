using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.Requests;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Services;

public interface IVacunoReferenceResolver
{
    Task<VacunoReferenceResolution> ResolveForCreateAsync(
        CreateVacunoRequest request,
        CancellationToken cancellationToken = default);

    Task<VacunoReferenceResolution> ResolveForUpdateAsync(
        long vacunoId,
        UpdateVacunoRequest request,
        CancellationToken cancellationToken = default);
}

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
