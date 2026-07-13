using FluentAssertions;
using FluentValidation.Results;
using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.Validators;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class UpdateVacunoInteractorTests
{
    private readonly Mock<IVacunoRepository> _repository = new();
    private readonly UpdateVacunoValidator _validator = new();

    [Fact]
    public async Task HandleAsync_WhenVacunoExists_ShouldUpdateEditableData()
    {
        var existing = CreateExistingVacuno();
        var command = CreateValidCommand(existing.Id) with 
        { 
            Nombre = "Nombre Editado",
            FechaNacimiento = existing.FechaNacimiento,
            TipoAdquisicionCode = existing.TipoAdquisicionCode
        };
        _repository.Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _repository.Setup(x => x.UpdateAsync(It.IsAny<Vacuno>(), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vacuno vacuno, decimal? _, string? _, CancellationToken _) => vacuno);
        var interactor = new UpdateVacunoInteractor(_repository.Object);

        var result = await interactor.HandleAsync(command, CancellationToken.None);

        result.Data.Codigo.Should().Be(existing.Codigo);
        result.Data.Nombre.Should().Be(command.Nombre);
        _repository.Verify(x => x.UpdateAsync(It.Is<Vacuno>(v => v.Nombre == command.Nombre), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenVacunoDoesNotExist_ShouldThrowNotFound()
    {
        var command = CreateValidCommand(99);
        _repository.Setup(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vacuno?)null);
        var interactor = new UpdateVacunoInteractor(_repository.Object);

        var act = async () => await interactor.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<VacunoNotFoundException>();
        _repository.Verify(x => x.UpdateAsync(It.IsAny<Vacuno>(), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Validator_WhenEditableFieldsAreInvalid_ShouldRejectCommand()
    {
        var command = CreateValidCommand() with
        {
            Nombre = string.Empty,
            TipoAdquisicionCode = string.Empty,
            RazaCode = string.Empty,
            ColorCode = string.Empty,
            SexoCode = string.Empty,
            GranjaId = 0,
            Observaciones = new string('X', 151)
        };

        var result = _validator.Validate(command);

        result.Should().Contain([
            "VACUNO-VACUNO-UPDATE-NOMBRE-NULL",
            "VACUNO-VACUNO-UPDATE-TIPO_ADQUISICION_CODE-NULL",
            "VACUNO-VACUNO-UPDATE-RAZA_CODE-NULL",
            "VACUNO-VACUNO-UPDATE-COLOR_CODE-NULL",
            "VACUNO-VACUNO-UPDATE-SEXO_CODE-NULL",
            "VACUNO-VACUNO-UPDATE-GRANJA_ID-INVALID",
            "VACUNO-VACUNO-UPDATE-OBSERVACIONES-INVALID"
        ]);
    }

    private static UpdateVacunoCommand CreateValidCommand(long id = 20) => new(
        Id: id,
        Nombre: "Vacuno Editado",
        FechaNacimiento: new DateOnly(2024, 1, 10),
        TipoAdquisicionCode: "NACIMIENTO",
        RazaCode: "HOLSTEIN",
        ColorCode: "NEGRO_BLANCO",
        SexoCode: "HEMBRA",
        PadreId: null,
        MadreId: null,
        GranjaId: 1,
        Observaciones: "Actualizacion de prueba",
        PrecioCompra: null,
        AptoPara: null);

    private static Vacuno CreateExistingVacuno() => Vacuno.Rehydrate(
        id: 20,
        codigo: "VAC-TST-020",
        nombre: "Vacuno Original",
        fechaNacimiento: new DateOnly(2023, 5, 1),
        tipoAdquisicionCode: "NACIMIENTO",
        razaCode: "HOLSTEIN",
        colorCode: "NEGRO_BLANCO",
        sexoCode: "HEMBRA",
        padreId: null,
        madreId: null,
        granjaId: 1,
        observaciones: null,
        fechaRegistro: new DateOnly(2023, 5, 2),
        createdAt: DateTime.UtcNow.AddDays(-1),
        updatedAt: DateTime.UtcNow.AddDays(-1),
        deletedAt: null,
        motivoEliminacion: null,
        createdBy: null,
        updatedBy: null,
        deletedBy: null);
}
