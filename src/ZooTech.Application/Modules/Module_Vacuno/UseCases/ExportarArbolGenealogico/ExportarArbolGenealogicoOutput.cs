using ZooTech.Application.Common.Gateway.Auditing;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public sealed record ExportarArbolGenealogicoOutput(
    byte[] Bytes,
    string ContentType,
    string FileName
) : IAuditResponseMetadataProvider
{
    public object GetAuditMetadata()
        => new ExportedFileAuditMetadata(
            FileName,
            Path.GetExtension(FileName).TrimStart('.'),
            ContentType,
            Bytes.Length,
            true);
}
