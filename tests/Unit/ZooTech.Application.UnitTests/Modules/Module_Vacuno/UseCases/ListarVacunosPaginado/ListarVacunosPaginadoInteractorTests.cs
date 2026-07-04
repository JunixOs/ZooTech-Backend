using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunosPaginado;
using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;
using ZooTech.Domain.ValueObjects;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class ListarVacunosPaginadoInteractorTests
{
    private readonly IVacunoRepository _repositoryMock;
    private readonly IListarVacunosPaginadoOutputPort _outputMock;
    private readonly IFeatureService _featuresMock;
    private readonly ITenantContext _tenantMock;
    private readonly IDateTimeProvider _timeMock;
    private readonly ListarVacunosPaginadoInteractor _sut;

    public ListarVacunosPaginadoInteractorTests()
    {
        _repositoryMock = Substitute.For<IVacunoRepository>();
        _outputMock = Substitute.For<IListarVacunosPaginadoOutputPort>();
        _featuresMock = Substitute.For<IFeatureService>();
        _tenantMock = Substitute.For<ITenantContext>();
        _timeMock = Substitute.For<IDateTimeProvider>();

        _sut = new ListarVacunosPaginadoInteractor(
            _repositoryMock,
            _outputMock,
            _featuresMock,
            _tenantMock,
            _timeMock);
    }

    [Fact]
    public async Task Handle_WhenFeatureDisabled_ReturnsErrorToOutputPort()
    {
        // Arrange
        _featuresMock.IsEnabledAsync("module.vacunos").Returns(false);
        var command = new ListarVacunosPaginadoCommand();

        // Act
        await _sut.Handle(command);

        // Assert
        await _outputMock.Received(1).Error("FEATURE_DISABLED", Arg.Any<string>());
        await _repositoryMock.DidNotReceiveWithAnyArgs().GetPagedAsync(default, default, default, default, default, default);
    }

    [Fact]
    public async Task Handle_WhenDatesAreNull_CalculatesDefault30DaysRange()
    {
        // Arrange
        _featuresMock.IsEnabledAsync("module.vacunos").Returns(true);
        var mockDate = new DateTime(2023, 10, 31, 12, 0, 0, DateTimeKind.Utc);
        _timeMock.UtcNow.Returns(mockDate);

        var command = new ListarVacunosPaginadoCommand { FechaDesde = null, FechaHasta = null };
        _repositoryMock.GetPagedAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<EstadoAnimal?>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns((new List<VacunoResumen>(), 0));

        // Act
        await _sut.Handle(command);

        // Assert
        var expectedHasta = mockDate.Date;
        var expectedDesde = expectedHasta.AddDays(-30);

        await _repositoryMock.Received(1).GetPagedAsync(
            expectedDesde,
            expectedHasta,
            Arg.Any<EstadoAnimal?>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>());
    }

    [Fact]
    public async Task Handle_WhenValidCommand_MapsDataAndReturnsOk()
    {
        // Arrange
        _featuresMock.IsEnabledAsync("module.vacunos").Returns(true);
        _timeMock.UtcNow.Returns(new DateTime(2023, 1, 1));

        var animalResumen = new VacunoResumen
        {
            Id = 1,
            Codigo = "VAC-01",
            Nombre = "Lola",
            FechaRegistro = new DateTime(2020, 1, 2),
            Raza = "Angus",
            Procedencia = "G1 - D1 - P1 - Dep1",
            Estado = EstadoAnimal.SANO
        };

        _repositoryMock.GetPagedAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<EstadoAnimal?>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns((new List<VacunoResumen> { animalResumen }, 1));

        var command = new ListarVacunosPaginadoCommand { Page = 2, Limit = 10 };

        // Act
        await _sut.Handle(command);

        // Assert
        await _outputMock.Received(1).Ok(Arg.Is<ListarVacunosPaginadoOutput>(o =>
            o.PagedData.Total == 1 &&
            o.PagedData.Page == 2 &&
            o.PagedData.PageSize == 10 &&
            o.PagedData.Data.Count == 1 &&
            o.PagedData.Data[0].Codigo == "VAC-01" &&
            o.PagedData.Data[0].Raza == "Angus"
        ));
    }
}
