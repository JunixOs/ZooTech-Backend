using System.Text.Json.Serialization;

namespace ZooTech.Application.Modules.Module_Vacuno.Services;

public interface IVacunoPhotoStorage
{
    Task<StoredVacunoPhoto> SaveAsync(
        VacunoPhotoUpload upload,
        CancellationToken cancellationToken = default);

    Task<VacunoPhotoContent?> ReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string relativePath,
        CancellationToken cancellationToken = default);
}

public sealed record VacunoPhotoUpload(
    string FileName,
    string ContentType,
    [property: JsonIgnore] byte[] Content)
{
    public const long MaxBytes = 5L * 1024 * 1024;
}

public sealed record StoredVacunoPhoto(
    string OriginalName,
    string StoredName,
    string RelativePath,
    string Extension,
    string ContentType,
    long SizeBytes,
    string Sha256);

public sealed record VacunoPhotoContent(
    string ContentType,
    byte[] Content);
