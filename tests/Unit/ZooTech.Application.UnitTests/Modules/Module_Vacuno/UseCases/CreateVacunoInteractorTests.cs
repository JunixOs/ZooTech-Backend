using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.Validators;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class CreateVacunoInteractorTests
{
    private readonly Mock<IVacunoRepository> _repository = new();
    private readonly CreateVacunoValidator _validator = new();

    [Fact]
    public async Task HandleAsync_WhenRequestIsValid_ShouldPersistVacuno()
    {
        var command = CreateValidCommand();
        _repository.Setup(x => x.ExistsCodigoAsync(command.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repository.Setup(x => x.AddAsync(It.IsAny<Vacuno>(), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vacuno vacuno, decimal? _, string? _, CancellationToken _) => RehydrateSaved(vacuno, id: 10));
        var interactor = new CreateVacunoInteractor(_repository.Object);

        var result = await interactor.HandleAsync(command, CancellationToken.None);

        result.Data.Id.Should().Be(10);
        result.Data.Codigo.Should().Be(command.Codigo);
        _repository.Verify(x => x.AddAsync(It.Is<Vacuno>(v => v.Codigo == command.Codigo), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCodigoAlreadyExists_ShouldThrowConflict()
    {
        var command = CreateValidCommand();
        _repository.Setup(x => x.ExistsCodigoAsync(command.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var interactor = new CreateVacunoInteractor(_repository.Object);

        var act = async () => await interactor.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<VacunoAlreadyExistsException>()
            .WithMessage("*código o datos repetidos*");
        _repository.Verify(x => x.AddAsync(It.IsAny<Vacuno>(), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Validator_WhenRequiredFieldsAreMissing_ShouldRejectCommand()
    {
        var command = CreateValidCommand() with
        {
            Codigo = string.Empty,
            Nombre = string.Empty,
            TipoAdquisicionCode = string.Empty,
            RazaCode = string.Empty,
            ColorCode = string.Empty,
            SexoCode = string.Empty,
            GranjaId = 0
        };

        var result = _validator.Validate(command);

        result.Should().Contain([
            "VACUNO-VACUNO-CREATE-CODIGO-NULL",
            "VACUNO-VACUNO-CREATE-NOMBRE-NULL",
            "VACUNO-VACUNO-CREATE-TIPO_ADQUISICION_CODE-NULL",
            "VACUNO-VACUNO-CREATE-RAZA_CODE-NULL",
            "VACUNO-VACUNO-CREATE-COLOR_CODE-NULL",
            "VACUNO-VACUNO-CREATE-SEXO_CODE-NULL",
            "VACUNO-VACUNO-CREATE-GRANJA_ID-INVALID"
        ]);
    }

    [Fact]
    public async Task Validator_WhenCodigoOrObservacionesExceedLimits_ShouldRejectCommand()
    {
        var command = CreateValidCommand() with
        {
            Codigo = new string('A', 16),
            Observaciones = new string('B', 151)
        };

        var result = _validator.Validate(command);

        result.Should().Contain([
            "VACUNO-VACUNO-CREATE-CODIGO-INVALID",
            "VACUNO-VACUNO-CREATE-OBSERVACIONES-INVALID"
        ]);
    }

    private static CreateVacunoCommand CreateValidCommand() => new(
        Codigo: "VAC-TST-001",
        Nombre: "Vacuno Test",
        FechaNacimiento: new DateOnly(2024, 1, 10),
        TipoAdquisicionCode: "NACIMIENTO",
        RazaCode: "HOLSTEIN",
        ColorCode: "NEGRO_BLANCO",
        SexoCode: "HEMBRA",
        PadreId: null,
        MadreId: null,
        GranjaId: 1,
        Observaciones: "Registro de prueba",
        PrecioCompra: null,
        AptoPara: null);

    private static Vacuno RehydrateSaved(Vacuno vacuno, long id) => Vacuno.Rehydrate(
        id,
        vacuno.Codigo,
        vacuno.Nombre,
        vacuno.FechaNacimiento,
        vacuno.TipoAdquisicionCode,
        vacuno.RazaCode,
        vacuno.ColorCode,
        vacuno.SexoCode,
        vacuno.PadreId,
        vacuno.MadreId,
        vacuno.GranjaId,
        vacuno.Observaciones,
        vacuno.FechaRegistro,
        vacuno.CreatedAt,
        vacuno.UpdatedAt,
        vacuno.DeletedAt,
        vacuno.MotivoEliminacion,
        vacuno.CreatedBy,
        vacuno.UpdatedBy,
        vacuno.DeletedBy);
}
