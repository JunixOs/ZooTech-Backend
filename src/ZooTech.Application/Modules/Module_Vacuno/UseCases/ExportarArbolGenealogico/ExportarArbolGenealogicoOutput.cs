namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;

public sealed record ExportarArbolGenealogicoOutput(
    byte[] Bytes,
    string ContentType,
    string FileName
);