using Moq;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.Services;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Vacuno.Support;

internal sealed class VacunoMutationTestContext
{
    public Mock<IVacunoRepository> Repository { get; } = new();
    public Mock<IGanaderiaUnitOfWork> UnitOfWork { get; } = new();
    public Mock<ITenantConfigurationProvider> Settings { get; } = new();
    public Mock<IVacunoReferenceResolver> ReferenceResolver { get; } = new();

    public VacunoMutationTestContext()
    {
        UnitOfWork.Setup(x => x.Vacunos).Returns(Repository.Object);
        UnitOfWork
            .Setup(x => x.ExecuteInTransactionAsync(
                It.IsAny<Func<CancellationToken, Task<Vacuno>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Func<Vacuno, CancellationToken, Task<Vacuno>>?>()))
            .Returns((Func<CancellationToken, Task<Vacuno>> operation, CancellationToken token,
                Func<Vacuno, CancellationToken, Task<Vacuno>>? _) => operation(token));

        ReferenceResolver
            .Setup(x => x.ResolveAsync(It.IsAny<VacunoReferenceData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(VacunoReferenceResolution.Ok(null, null, 1));

        SetValidationLimits();
    }

    public CreateVacunoInteractor CreateInteractor() => new(
        UnitOfWork.Object,
        Settings.Object,
        ReferenceResolver.Object);

    public UpdateVacunoInteractor UpdateInteractor() => new(
        UnitOfWork.Object,
        Settings.Object,
        ReferenceResolver.Object);

    private void SetValidationLimits()
    {
        Settings.Setup(x => x.GetSettingAsync(ZooTech.Domain.Configuration.Settings.Vacunos.VacunosCodigoMaxLength))
            .ReturnsAsync(20);
        Settings.Setup(x => x.GetSettingAsync(ZooTech.Domain.Configuration.Settings.Vacunos.VacunosInputMaxLength))
            .ReturnsAsync(100);
        Settings.Setup(x => x.GetSettingAsync(ZooTech.Domain.Configuration.Settings.Vacunos.VacunosObservacionesMaxLength))
            .ReturnsAsync(150);
        Settings.Setup(x => x.GetSettingAsync(ZooTech.Domain.Configuration.Settings.Vacunos.VacunosObservacionesMaxWords))
            .ReturnsAsync(30);
        Settings.Setup(x => x.GetSettingAsync(ZooTech.Domain.Configuration.Settings.Vacunos.VacunosCodigoUppercaseRequired))
            .ReturnsAsync(true);
    }
}
