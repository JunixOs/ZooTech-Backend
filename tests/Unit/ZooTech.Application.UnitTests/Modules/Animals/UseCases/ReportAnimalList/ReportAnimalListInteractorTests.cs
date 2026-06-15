using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

namespace ZooTech.Application.UnitTests.Modules.Animals.UseCases.ReportAnimalList;

public class ReportAnimalListInteractorTests
{
    private class FakeAnimalReportRepository : IAnimalReportRepository
    {
        public Task<IReadOnlyCollection<ReportAnimalListItem>> GetAnimalListAsync(ReportAnimalListFilter filter, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<ReportAnimalListItem> items = new List<ReportAnimalListItem>
            {
                new ReportAnimalListItem("V001", "Lola", "Holstein", "Hembra", "Nacimiento", "Activo", new DateOnly(2023, 1, 15))
            };
            return Task.FromResult(items);
        }
    }

    private class FakeExcelService : IAnimalReportExcelService
    {
        public byte[] GenerateAnimalListExcel(ReportAnimalListOutput output) => Array.Empty<byte>();
    }

    private class FakePdfService : IAnimalReportPdfService
    {
        public byte[] GenerateAnimalListPdf(ReportAnimalListOutput output) => new byte[] { 1, 2, 3 };
    }

    private class FakeOutputPort : IReportAnimalListOutputPort
    {
        public ReportAnimalListPdfOutput? ReceivedPdfOutput { get; private set; }
        public ReportAnimalListExcelOutput? ReceivedExcelOutput { get; private set; }
        public ReportAnimalListOutput? ReceivedListOutput { get; private set; }

        public void PresentList(ReportAnimalListOutput output) => ReceivedListOutput = output;
        public void PresentExcel(ReportAnimalListExcelOutput output) => ReceivedExcelOutput = output;
        public void PresentPdf(ReportAnimalListPdfOutput output) => ReceivedPdfOutput = output;
        public void PresentValidationError(ReportAnimalListValidationException exception) {}
        public void PresentNotFound(AnimalReportNotFoundException exception) {}
        public void PresentUnexpectedError(AnimalReportGenerationException exception) {}
    }

    [Fact]
    public async Task Handle_WithExportPdfTrue_ShouldPresentPdfOutput()
    {
        // Arrange
        var repository = new FakeAnimalReportRepository();
        var excelService = new FakeExcelService();
        var pdfService = new FakePdfService();
        var validator = new ReportAnimalListValidator();
        var interactor = new ReportAnimalListInteractor(repository, excelService, pdfService, validator);

        var command = new ReportAnimalListCommand(
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 6, 1),
            null,
            ExportExcel: false,
            ExportPdf: true);

        var outputPort = new FakeOutputPort();

        // Act
        await interactor.Handle(command, outputPort);

        // Assert
        Assert.NotNull(outputPort.ReceivedPdfOutput);
        Assert.Equal("application/pdf", outputPort.ReceivedPdfOutput.ContentType);
        Assert.Equal(new byte[] { 1, 2, 3 }, outputPort.ReceivedPdfOutput.Content);
        Assert.StartsWith("reporte-vacunos-", outputPort.ReceivedPdfOutput.FileName);
        Assert.EndsWith(".pdf", outputPort.ReceivedPdfOutput.FileName);
    }
}
