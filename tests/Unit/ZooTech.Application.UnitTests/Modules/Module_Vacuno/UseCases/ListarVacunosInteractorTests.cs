using Moq;
using FluentAssertions;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.ListarVacuno;
using ZooTech.Application.Common.Gateway.Parametrization;
using MongoDB.Driver;
using ZooTech.Domain.Configuration;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class ListarVacunosInteractorTests
{
    private readonly Mock<IVacunoRepository> _vacunoRepositoryMock;
    private readonly Mock<ITenantConfigurationProvider> _settingsMock;
    private readonly ListarVacunosInteractor _interactor;

    public ListarVacunosInteractorTests()
    {
        _vacunoRepositoryMock = new Mock<IVacunoRepository>();
        _settingsMock = new Mock<ITenantConfigurationProvider>();
        
        _settingsMock.Setup(x => x.GetSettingAsync(Settings.Vacunos.VacunosDefaultFilterDays)).ReturnsAsync(30);

        _interactor = new ListarVacunosInteractor(_vacunoRepositoryMock.Object, _settingsMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenValidRequest_ReturnsPagedDataSuccessfully()
    {
        var fakeItems = new List<VacunoListItem>
        {
            new VacunoListItem(1L, "V-001", "Lola", DateOnly.Parse("2020-01-01"), "R01", "Granja A", false, DateOnly.FromDateTime(DateTime.UtcNow))
        };

        _vacunoRepositoryMock.Setup(x => x.GetPagedAsync(
                It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((fakeItems, 1));

        var command = new ListarVacunosCommand(Page: 1, Limit: 10);

        var result = await _interactor.HandleAsync(command);

        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items.First().Codigo.Should().Be("V-001");
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrowsException_ThrowsException()
    {
        _vacunoRepositoryMock.Setup(x => x.GetPagedAsync(
                It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var command = new ListarVacunosCommand();

        var act = async () => await _interactor.HandleAsync(command);

        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}
