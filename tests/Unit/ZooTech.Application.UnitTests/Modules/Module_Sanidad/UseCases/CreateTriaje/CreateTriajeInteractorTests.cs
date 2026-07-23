using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;
using ZooTech.Application.UnitTests.Modules.Module_Sanidad;
using ZooTech.Domain.Common.Interfaces;
using ZooTech.Domain.Module_Sanidad.Entities;
using ZooTech.Domain.Module_Sanidad.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.CreateTriaje;

public class CreateTriajeInteractorTests
{
    private readonly Mock<ITriajeRepository> _repositoryMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly Mock<IEstadoRegistroRepository> _estadoRegistroRepositoryMock = new();

    [Fact]
    public async Task Handle_WhenCommandIsValid_CreatesTriaje()
    {
        var now = DateTime.UtcNow;
        _dateTimeProviderMock.Setup(p => p.ServerNow).Returns(now);
        _estadoRegistroRepositoryMock.Setup(r => r.GetActiveCodeAsync(It.IsAny<CancellationToken>())).ReturnsAsync("ACTIVO");
        _repositoryMock.Setup(r => r.ExistsVacunoAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsUsuarioAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsTipoPesoAsync("CONTROL", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.GenerateCodigoAsync(It.IsAny<CancellationToken>())).ReturnsAsync("TRI010");
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Triaje>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Triaje t, CancellationToken _) => SanidadTestData.CreateTriaje(
                id: 10,
                codigo: t.Codigo,
                vacunoId: t.VacunoId,
                tipoPesoCode: t.TipoPesoCode,
                pesoKg: t.PesoKg,
                observaciones: t.Observaciones,
                estadoRegistroCode: t.EstadoRegistroCode,
                encargadoUsuarioId: t.EncargadoUsuarioId,
                fechaHora: t.FechaHora,
                createdAt: t.CreatedAt));
        var interactor = CreateInteractor();

        var result = await interactor.Handle(new CreateTriajeCommand
        {
            VacunoId = 1,
            TipoPesoCode = "CONTROL",
            PesoKg = 120m,
            Observaciones = "Sin observaciones",
            EncargadoUsuarioId = 10,
            FechaHora = now.AddDays(-1),
        });

        Assert.Equal(10, result.Id);
        Assert.Equal("TRI010", result.Codigo);
        Assert.Equal(now, result.FechaHora);
        Assert.Equal("ACTIVO", result.EstadoRegistroCode);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<Triaje>(t =>
            t.Codigo == "TRI010" &&
            t.FechaHora == now &&
            t.TipoPesoCode == "CONTROL" &&
            t.PesoKg == 120m), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenVacunoDoesNotExist_ThrowsConflictException()
    {
        _repositoryMock.Setup(r => r.ExistsVacunoAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var interactor = CreateInteractor();

        await Assert.ThrowsAsync<ConflictException>(() => interactor.Handle(ValidCommand()));
    }

    [Fact]
    public async Task Handle_WhenEncargadoDoesNotExist_ThrowsConflictException()
    {
        _repositoryMock.Setup(r => r.ExistsVacunoAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsUsuarioAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var interactor = CreateInteractor();

        await Assert.ThrowsAsync<ConflictException>(() => interactor.Handle(ValidCommand()));
    }

    [Fact]
    public async Task Handle_WhenTipoPesoDoesNotExist_ThrowsConflictException()
    {
        _repositoryMock.Setup(r => r.ExistsVacunoAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsUsuarioAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsTipoPesoAsync("CONTROL", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var interactor = CreateInteractor();

        await Assert.ThrowsAsync<ConflictException>(() => interactor.Handle(ValidCommand()));
    }

    private CreateTriajeInteractor CreateInteractor()
        => new(_repositoryMock.Object, _dateTimeProviderMock.Object, _estadoRegistroRepositoryMock.Object);

    private static CreateTriajeCommand ValidCommand()
        => new()
        {
            VacunoId = 1,
            TipoPesoCode = "CONTROL",
            PesoKg = 120m,
            Observaciones = "Sin observaciones",
            EncargadoUsuarioId = 10,
            FechaHora = DateTime.UtcNow,
        };
}
