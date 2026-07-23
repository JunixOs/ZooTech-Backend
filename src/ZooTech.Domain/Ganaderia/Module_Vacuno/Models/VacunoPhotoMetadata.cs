namespace ZooTech.Domain.Ganaderia.Module_Vacuno.Models;

public sealed record VacunoPhotoMetadata(
    string OriginalName,
    string StoredName,
    string RelativePath,
    string Extension,
    string ContentType,
    long SizeBytes,
    string Sha256);
