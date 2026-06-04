namespace ZooTech.Infrastructure.Storage;

public class ReportStorageOptions
{
    public string ReportesBasePath { get; set; } = "wwwroot/reportes";
    public string ReportesVacunosPath { get; set; } = "vacunos";
    public string ReportesUrlBase { get; set; } = "/reportes/vacunos/";
}
