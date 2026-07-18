namespace ZooTech.Application.Common.Gateway.Export;

public interface IPdfDocumentGenerator
{
    byte[] Generate(ReportExportRequest request);
}
