using FluentAssertions;
using Moq;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Vacuno.Services;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.Validators;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.UseCases;

public class CreateVacunoInteractorTests
{
    private readonly Mock<IVacunoRepository> _repository = new();
    private readonly Mock<IGanaderiaUnitOfWork> _unitOfWork = new();
    private readonly Mock<IAppCacheService> _cache = new();
    private readonly Mock<ITenantConfigurationProvider> _tenantConfigurationProvider = new();
    private readonly Mock<IVacunoReferenceResolver> _referenceResolver = new();
    private readonly CreateVacunoValidator _validator = new();

    public CreateVacunoInteractorTests()
    {
        _unitOfWork.Setup(x => x.Vacunos).Returns(_repository.Object);
        _unitOfWork
            .Setup(x => x.ExecuteInTransactionAsync(
                It.IsAny<Func<CancellationToken, Task<Vacuno>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Func<Vacuno, CancellationToken, Task<Vacuno>>?>()))
            .Returns((Func<CancellationToken, Task<Vacuno>> operation, CancellationToken cancellationToken, Func<Vacuno, CancellationToken, Task<Vacuno>>? _) =>
                operation(cancellationToken));
        _referenceResolver
            .Setup(x => x.ResolveAsync(It.IsAny<VacunoReferenceData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(VacunoReferenceResolution.Ok(null, null, 1));
        SetupValidationSettings();
    }

    [Fact]
    public async Task HandleAsync_WhenRequestIsValid_ShouldPersistVacuno()
    {
        var command = CreateValidCommand();
        _repository.Setup(x => x.ExistsCodigoAsync(command.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repository.Setup(x => x.AddAsync(It.IsAny<Vacuno>(), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vacuno vacuno, decimal? _, string? _, CancellationToken _) => RehydrateSaved(vacuno, id: 10));
        var interactor = CreateInteractor();

        var result = await interactor.HandleAsync(command, CancellationToken.None);

        result.Data.Id.Should().Be(10);
        result.Data.Codigo.Should().Be(command.Codigo);
        _repository.Verify(x => x.AddAsync(It.Is<Vacuno>(v => v.Codigo == command.Codigo), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
        _cache.Verify(x => x.RemoveByPrefixAsync("vacunos:listar"), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCodigoAlreadyExists_ShouldThrowConflict()
    {
        var command = CreateValidCommand();
        _repository.Setup(x => x.ExistsCodigoAsync(command.Codigo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var interactor = CreateInteractor();

        var act = async () => await interactor.HandleAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<VacunoAlreadyExistsException>()
            .WithMessage("*codigo o datos repetidos*");
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
            GranjaId = 0,
            Granja = null,
            CodigoDistrito = null
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
    public async Task HandleAsync_WhenTenantLimitsAreExceeded_ShouldThrowValidation()
    {
        var command = CreateValidCommand() with
        {
            Codigo = new string('A', 16)
        };
        _tenantConfigurationProvider
            .Setup(x => x.GetSettingAsync(Settings.Vacunos.VacunosCodigoMaxLength))
            .ReturnsAsync(15);
        var interactor = CreateInteractor();

        var act = async () => await interactor.HandleAsync(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Message.Should().Contain("15 caracteres");
        exception.Which.FieldErrors.Should().ContainSingle(error => error.Field == "codigo");
        _repository.Verify(x => x.ExistsCodigoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static CreateVacunoCommand CreateValidCommand() => new(
        Codigo: "VAC-TST-001",
        Nombre: "Vacuno Test",
        FechaNacimiento: new DateOnly(2024, 1, 10),
        TipoAdquisicionCode: "NACIMIENTO",
        RazaCode: "HOLSTEIN",
        ColorCode: "NEGRO_BLANCO",
        SexoCode: "HEMBRA",
        CodigoPadre: null,
        CodigoMadre: null,
        GranjaId: 1,
        Granja: null,
        CodigoDistrito: null,
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

    private CreateVacunoInteractor CreateInteractor()
        => new(
            _unitOfWork.Object,
            _cache.Object,
            _tenantConfigurationProvider.Object,
            _referenceResolver.Object);

    private void SetupValidationSettings()
    {
        _tenantConfigurationProvider
            .Setup(x => x.GetSettingAsync(Settings.Vacunos.VacunosCodigoMaxLength))
            .ReturnsAsync(20);
        _tenantConfigurationProvider
            .Setup(x => x.GetSettingAsync(Settings.Vacunos.VacunosInputMaxLength))
            .ReturnsAsync(100);
        _tenantConfigurationProvider
            .Setup(x => x.GetSettingAsync(Settings.Vacunos.VacunosObservacionesMaxLength))
            .ReturnsAsync(150);
        _tenantConfigurationProvider
            .Setup(x => x.GetSettingAsync(Settings.Vacunos.VacunosObservacionesMaxWords))
            .ReturnsAsync(30);
        _tenantConfigurationProvider
            .Setup(x => x.GetSettingAsync(Settings.Vacunos.VacunosCodigoUppercaseRequired))
            .ReturnsAsync(true);
    }
}
