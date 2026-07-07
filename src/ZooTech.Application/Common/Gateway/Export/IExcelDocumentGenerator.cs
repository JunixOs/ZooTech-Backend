namespace ZooTech.Application.Common.Gateway.Export;

public interface IExcelDocumentGenerator
{
    byte[] Generate(ReportExportRequest request);
}
