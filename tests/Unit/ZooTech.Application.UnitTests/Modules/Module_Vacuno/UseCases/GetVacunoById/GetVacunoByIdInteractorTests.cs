using FluentAssertions;
using NSubstitute;
using Xunit;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.GetVacunoById;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases.GetVacunoById;

public class GetVacunoByIdInteractorTests
{
    private readonly IVacunoRepository _vacunoRepository;
    private readonly GetVacunoByIdInteractor _sut;

    public GetVacunoByIdInteractorTests()
    {
        _vacunoRepository = Substitute.For<IVacunoRepository>();
        _sut = new GetVacunoByIdInteractor(_vacunoRepository);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoExists_ReturnsVacunoOutput()
    {
        // Arrange
        var command = new GetVacunoByIdCommand(1);
        var expectedVacuno = Vacuno.Rehydrate(
            id: 1,
            codigo: "VAC001",
            nombre: "Lola",
            fechaNacimiento: new DateOnly(2020, 1, 1),
            tipoAdquisicionCode: "C",
            razaCode: "H",
            colorCode: "BN",
            sexoCode: "H",
            padreId: null,
            madreId: null,
            granjaId: 1,
            observaciones: null,
            fechaRegistro: new DateOnly(2020, 1, 1),
            createdAt: DateTime.UtcNow,
            updatedAt: DateTime.UtcNow,
            deletedAt: null,
            motivoEliminacion: null,
            createdBy: 1,
            updatedBy: 1,
            deletedBy: null
        );

        _vacunoRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expectedVacuno);

        // Act
        var result = await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(1);
        result.Data.Codigo.Should().Be("VAC001");
        result.Data.Nombre.Should().Be("Lola");
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoDoesNotExist_ThrowsVacunoNotFoundException()
    {
        // Arrange
        var command = new GetVacunoByIdCommand(999);
        _vacunoRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Vacuno?)null);

        // Act
        Func<Task> act = async () => await _sut.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<VacunoNotFoundException>()
            .WithMessage($"No existe un vacuno con el ID 999.");
    }
}
