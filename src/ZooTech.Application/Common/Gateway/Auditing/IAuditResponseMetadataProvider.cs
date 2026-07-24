namespace ZooTech.Application.Common.Gateway.Auditing;

public interface IAuditResponseMetadataProvider
{
    object GetAuditMetadata();
}

public sealed record ExportedFileAuditMetadata(
    string FileName,
    string Format,
    string ContentType,
    int SizeBytes,
    bool Success);
