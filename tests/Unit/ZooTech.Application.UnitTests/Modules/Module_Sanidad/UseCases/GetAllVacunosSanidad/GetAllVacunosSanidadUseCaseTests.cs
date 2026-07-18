using Moq;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.UnitTests.Modules.Module_Sanidad.UseCases.GetAllVacunosSanidad;

public class GetAllVacunosSanidadUseCaseTests
{
    private readonly Mock<IVacunoRepository> _repositoryMock;

    public GetAllVacunosSanidadUseCaseTests()
    {
        _repositoryMock = new Mock<IVacunoRepository>();
    }

    [Fact]
    public async Task ExecuteAsync_DebeRetornarListaDeVacunos()
    {
        // Arrange
        var vacunos = new List<Vacuno>
        {
            CreateVacuno(1, "VAC001", "Estrella"),
            CreateVacuno(2, "VAC002", "Luna"),
            CreateVacuno(3, "VAC003", "Toro Rey")
        };
        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(vacunos);
        var useCase = new GetAllVacunosSanidadInteractor(_repositoryMock.Object);

        // Act
        var result = await useCase.Handle(EmptyCommand.Value(AuditEventType.Read, "Get all vacunos sanidad"));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("VAC001", result.Items.First().Codigo);
        Assert.Equal("Estrella", result.Items.First().Nombre);
    }

    [Fact]
    public async Task ExecuteAsync_CuandoNoHayVacunos_DebeRetornarListaVacia()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Vacuno>());
        var useCase = new GetAllVacunosSanidadInteractor(_repositoryMock.Object);

        // Act
        var result = await useCase.Handle(EmptyCommand.Value(AuditEventType.Read, "Get all vacunos sanidad"));

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    private static Vacuno CreateVacuno(long id, string codigo, string nombre)
    {
        var now = DateTime.UtcNow;

        return Vacuno.Rehydrate(
            id: id,
            codigo: codigo,
            nombre: nombre,
            fechaNacimiento: DateOnly.FromDateTime(now.AddYears(-2)),
            tipoAdquisicionCode: "NACIMIENTO",
            razaCode: "HOLSTEIN",
            colorCode: "NEGRO",
            sexoCode: "HEMBRA",
            padreId: null,
            madreId: null,
            granjaId: 1,
            observaciones: null,
            fechaRegistro: DateOnly.FromDateTime(now),
            createdAt: now,
            updatedAt: now,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: null,
            updatedBy: null,
            deletedBy: null);
    }
}
