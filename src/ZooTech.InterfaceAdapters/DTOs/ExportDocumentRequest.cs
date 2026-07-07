namespace ZooTech.InterfaceAdapters.DTOs;

public class ExportDocumentRequest
{
    public string Title { get; set; } = string.Empty;

    public List<ExportColumnRequest> Columns { get; set; } = new();

    public List<Dictionary<string, object?>> Rows { get; set; } = new();
}

public class ExportColumnRequest
{
    public string Header { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;
}
