using Moq;
using FluentAssertions;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.ListarVacuno;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class ListarVacunosInteractorTests
{
    private readonly Mock<IVacunoRepository> _vacunoRepositoryMock;
    private readonly Mock<IAppCacheService> _cacheMock;
    private readonly ListarVacunosInteractor _interactor;

    public ListarVacunosInteractorTests()
    {
        _vacunoRepositoryMock = new Mock<IVacunoRepository>();
        _cacheMock = new Mock<IAppCacheService>();
        _cacheMock
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<ListarVacunosOutput>>>()))
            .Returns((string _, Func<Task<ListarVacunosOutput>> factory) => factory());

        _interactor = new ListarVacunosInteractor(
            _vacunoRepositoryMock.Object,
            _cacheMock.Object);
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

        var command = new ListarVacunosQuery(Page: 1, Limit: 10);

        var result = await _interactor.HandleAsync(command);

        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items.First().Codigo.Should().Be("V-001");
        _cacheMock.Verify(x => x.GetOrCreateAsync(
            It.Is<string>(key => key.StartsWith("vacunos:listar")),
            It.IsAny<Func<Task<ListarVacunosOutput>>>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithoutDates_DoesNotApplyAnImplicitDateFilter()
    {
        _vacunoRepositoryMock.Setup(x => x.GetPagedAsync(
                It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(([], 0));

        await _interactor.HandleAsync(new ListarVacunosQuery());

        _vacunoRepositoryMock.Verify(x => x.GetPagedAsync(
            null,
            null,
            null,
            null,
            1,
            20,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrowsException_ThrowsException()
    {
        _vacunoRepositoryMock.Setup(x => x.GetPagedAsync(
                It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var command = new ListarVacunosQuery();

        var act = async () => await _interactor.HandleAsync(command);

        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}
