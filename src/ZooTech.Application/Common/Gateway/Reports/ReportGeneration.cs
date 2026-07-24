using ZooTech.Application.Common.Gateway.Auditing;

namespace ZooTech.Application.Common.Gateway.Reports;

public enum ReportFileFormat
{
    Excel,
    Pdf
}

public sealed record GeneratedReportDocument(
    byte[] Content,
    string ContentType,
    string Extension,
    string FileName) : IAuditResponseMetadataProvider
{
    public object GetAuditMetadata()
        => new ExportedFileAuditMetadata(
            FileName,
            Extension.TrimStart('.'),
            ContentType,
            Content.Length,
            true);
}

public interface IReportStrategy<in TModel>
{
    ReportFileFormat Format { get; }

    Task<GeneratedReportDocument> GenerateAsync(
        TModel model,
        CancellationToken cancellationToken = default);
}

public interface IReportStrategyResolver<in TModel>
{
    IReportStrategy<TModel> Resolve(ReportFileFormat format);
}

public sealed class ReportStrategyResolver<TModel> : IReportStrategyResolver<TModel>
{
    private readonly IReadOnlyDictionary<ReportFileFormat, IReportStrategy<TModel>> _strategies;

    public ReportStrategyResolver(IEnumerable<IReportStrategy<TModel>> strategies)
    {
        var registeredStrategies = strategies.ToList();
        var duplicate = registeredStrategies
            .GroupBy(strategy => strategy.Format)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicate is not null)
        {
            throw new InvalidOperationException(
                $"Existe más de una estrategia registrada para el formato {duplicate.Key}.");
        }

        _strategies = registeredStrategies.ToDictionary(strategy => strategy.Format);
    }

    public IReportStrategy<TModel> Resolve(ReportFileFormat format)
        => _strategies.TryGetValue(format, out var strategy)
            ? strategy
            : throw new InvalidOperationException(
                $"No existe una estrategia registrada para el formato {format}.");
}
