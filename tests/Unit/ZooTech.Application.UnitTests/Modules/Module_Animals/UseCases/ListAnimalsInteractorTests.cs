using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Features;
using ZooTech.Application.Common.Gateway.Repositories;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Animals.UseCases.ListAnimals;
using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;
using ZooTech.Domain.ValueObjects;

namespace ZooTech.Application.UnitTests.Modules.Module_Animals.UseCases;

public class ListAnimalsInteractorTests
{
    private readonly IAnimalRepository _repositoryMock;
    private readonly IListAnimalsOutputPort _outputMock;
    private readonly IFeatureService _featuresMock;
    private readonly ITenantContext _tenantMock;
    private readonly IDateTimeProvider _timeMock;
    private readonly ListAnimalsInteractor _sut;

    public ListAnimalsInteractorTests()
    {
        _repositoryMock = Substitute.For<IAnimalRepository>();
        _outputMock = Substitute.For<IListAnimalsOutputPort>();
        _featuresMock = Substitute.For<IFeatureService>();
        _tenantMock = Substitute.For<ITenantContext>();
        _timeMock = Substitute.For<IDateTimeProvider>();

        _sut = new ListAnimalsInteractor(
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
        var command = new ListAnimalsCommand();

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
        
        var command = new ListAnimalsCommand { FechaDesde = null, FechaHasta = null };
        _repositoryMock.GetPagedAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<EstadoAnimal?>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns((new List<Animal>(), 0));

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
        
        var animal = Animal.Create(
            AnimalId.Of(1),
            "VAC-01",
            "Lola",
            new DateTime(2020, 1, 1),
            new DateTime(2020, 1, 2),
            new Raza("ANG", "Angus"),
            new Procedencia("G1", "D1", "P1", "Dep1"),
            new DateTime(2020, 1, 2)
        );

        _repositoryMock.GetPagedAsync(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<EstadoAnimal?>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns((new List<Animal> { animal }, 1));

        var command = new ListAnimalsCommand { Page = 2, Limit = 10 };

        // Act
        await _sut.Handle(command);

        // Assert
        await _outputMock.Received(1).Ok(Arg.Is<ListAnimalsOutput>(o => 
            o.PagedData.Total == 1 &&
            o.PagedData.Page == 2 &&
            o.PagedData.PageSize == 10 &&
            o.PagedData.Data.Count == 1 &&
            o.PagedData.Data[0].Codigo == "VAC-01" &&
            o.PagedData.Data[0].Raza == "Angus"
        ));
    }
}
