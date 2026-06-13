using Moq;
using FluentAssertions;
using Xunit;
using ZooTech.Application.Common.Configuration;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Domain.Module_Vacuno.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class ListarVacunosInteractorTests
{
    private readonly Mock<IVacunoRepository> _vacunoRepositoryMock;
    private readonly Mock<IVacunosConfiguration> _settingsMock;
    private readonly ListarVacunosInteractor _interactor;

    public ListarVacunosInteractorTests()
    {
        _vacunoRepositoryMock = new Mock<IVacunoRepository>();
        _settingsMock = new Mock<IVacunosConfiguration>();
        
        _settingsMock.Setup(x => x.DefaultFilterDays).Returns(30);

        _interactor = new ListarVacunosInteractor(_vacunoRepositoryMock.Object, _settingsMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenValidRequest_ReturnsPagedDataSuccessfully()
    {
        var fakeVacunos = new List<(Vacuno Vacuno, string? Procedencia)>
        {
            (Vacuno.Rehydrate(1L, "V-001", "Lola", DateOnly.Parse("2020-01-01"), "C01", "R01", "C01", "HEMBRA", null, null, 1L, null, DateOnly.FromDateTime(DateTime.UtcNow), DateTime.UtcNow, DateTime.UtcNow, null, null, null, null, null), "Granja A")
        };

        _vacunoRepositoryMock.Setup(x => x.GetPagedAsync(
                It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((fakeVacunos, 1));

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
                It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var command = new ListarVacunosCommand();

        var act = async () => await _interactor.HandleAsync(command);

        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}
