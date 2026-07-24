using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Infrastructure.Storage;

namespace ZooTech.Infrastructure.UnitTests.Reports.Vacunos;

public sealed class VacunoReportFileStorageTests : IDisposable
{
    private readonly string _testRoot =
        Path.Combine(Path.GetTempPath(), $"zootech-report-storage-{Guid.NewGuid():N}");

    [Fact]
    public async Task SaveAndRead_KeepFilesIsolatedByTenant()
    {
        var tenantOneStorage = CreateStorage(1);
        var tenantTwoStorage = CreateStorage(2);

        var stored = await tenantOneStorage.SaveAsync(
            "listado",
            ".pdf",
            "application/pdf",
            [1, 2, 3]);

        var ownerRead = await tenantOneStorage.ReadAsync(stored.FileName);
        var otherTenantRead = await tenantTwoStorage.ReadAsync(stored.FileName);

        ownerRead.Should().NotBeNull();
        ownerRead!.Content.Should().Equal(1, 2, 3);
        otherTenantRead.Should().BeNull();
    }

    [Theory]
    [InlineData("../secret.pdf")]
    [InlineData("..\\secret.pdf")]
    [InlineData("folder/report.pdf")]
    public async Task ReadAsync_WhenFileNameAttemptsTraversal_ReturnsNull(string fileName)
    {
        var storage = CreateStorage(1);

        var result = await storage.ReadAsync(fileName);

        result.Should().BeNull();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
        {
            Directory.Delete(_testRoot, true);
        }
    }

    private VacunoReportFileStorage CreateStorage(int tenantId)
    {
        var context = new Mock<ITenantContext>();
        context.SetupGet(value => value.TenantId).Returns(tenantId);

        return new VacunoReportFileStorage(
            Options.Create(new ReportStorageOptions
            {
                ReportesBasePath = _testRoot,
                ReportesVacunosPath = "vacunos"
            }),
            context.Object);
    }
}
