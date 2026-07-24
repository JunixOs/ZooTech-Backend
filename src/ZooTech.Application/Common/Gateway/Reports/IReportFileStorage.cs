namespace ZooTech.Application.Common.Gateway.Reports;

public interface IReportFileStorage
{
    Task<StoredReportFile> SaveAsync(
        string filePrefix,
        string extension,
        string contentType,
        byte[] content,
        CancellationToken cancellationToken = default);

    Task<StoredReportFileContent?> ReadAsync(
        string fileName,
        CancellationToken cancellationToken = default);
}

public sealed record StoredReportFile(
    string FileName,
    string DownloadUrl);

public sealed record StoredReportFileContent(
    string FileName,
    string ContentType,
    byte[] Content);
