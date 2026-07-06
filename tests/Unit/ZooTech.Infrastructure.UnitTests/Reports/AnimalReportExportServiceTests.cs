using System.IO.Compression;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;
using ZooTech.Infrastructure.Reports;

namespace ZooTech.Infrastructure.UnitTests.Reports;

public class AnimalReportExportServiceTests
{
    [Fact]
    public void GenerateAnimalListExcel_ShouldGenerateNonEmptyFileWithNewHeaders()
    {
        var service = new AnimalReportExcelService();

        var content = service.GenerateAnimalListExcel(CreateOutput());

        Assert.NotEmpty(content);
        using var stream = new MemoryStream(content);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        var sharedStringsEntry = archive.GetEntry("xl/sharedStrings.xml");
        Assert.NotNull(sharedStringsEntry);
        using var sharedStringsStream = sharedStringsEntry.Open();
        using var reader = new StreamReader(sharedStringsStream);
        var sharedStrings = reader.ReadToEnd();
        Assert.Contains("Fecha nacimiento", sharedStrings);
        Assert.Contains("Tipo adquisicion", sharedStrings);
        Assert.Contains("Color", sharedStrings);
        Assert.Contains("Granja", sharedStrings);
    }

    [Fact]
    public void GenerateAnimalListPdf_ShouldGenerateNonEmptyFile()
    {
        var service = new AnimalReportPdfService();

        var content = service.GenerateAnimalListPdf(CreateOutput());

        Assert.NotEmpty(content);
        Assert.True(content.Length > 100);
    }

    [Fact]
    public void AnimalReportPdfService_Source_ShouldNotContainMojibakeHeaders()
    {
        var source = File.ReadAllText(Path.Combine(
            FindRepositoryRoot(),
            "src",
            "ZooTech.Infrastructure",
            "Reports",
            "AnimalReportPdfService.cs"));

        Assert.Contains("Código", source);
        Assert.Contains("Página", source);
        Assert.DoesNotContain("CÃ", source);
        Assert.DoesNotContain("PÃ", source);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "ZooTech Backend - Solution.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new DirectoryNotFoundException("No se encontro la raiz del repositorio.");
    }

    private static ReportAnimalListOutput CreateOutput()
    {
        return new ReportAnimalListOutput(
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 31),
            "Lola",
            "HOL",
            "NEG",
            "H",
            "NAC",
            1,
            "ACT",
            new[]
            {
                new ReportAnimalListItem(
                    "V001",
                    "Lola",
                    new DateOnly(2021, 5, 10),
                    "Nacimiento",
                    "Holstein",
                    "Negro",
                    "Hembra",
                    "Granja Norte",
                    "Activo",
                    new DateOnly(2023, 1, 15))
            });
    }
}
