namespace ZooTech.Infrastructure.Storage;

public sealed class ReportStorageOptions
{
    public string ReportesBasePath { get; set; } = "storage";
    public string ReportesVacunosPath { get; set; } = "reportes/vacunos";
    public string VacunoMediaRoot { get; set; } = "wwwroot";
}
