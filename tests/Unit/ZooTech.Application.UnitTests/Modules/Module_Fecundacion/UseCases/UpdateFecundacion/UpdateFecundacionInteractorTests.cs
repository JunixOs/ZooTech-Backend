using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public sealed class UpdateFecundacionInteractorTests
{
    private readonly IFecundacionRepository _repository = Substitute.For<IFecundacionRepository>();
    private readonly IAppCacheService _cache = Substitute.For<IAppCacheService>();
    private readonly IGanaderiaUnitOfWork _unitOfWork = Substitute.For<IGanaderiaUnitOfWork>();
    private readonly UpdateFecundacionInteractor _sut;

    public UpdateFecundacionInteractorTests()
    {
        _unitOfWork.Fecundaciones.Returns(_repository);
        _sut = new UpdateFecundacionInteractor(_unitOfWork, _cache);
    }

    [Fact]
    public async Task HandleAsync_DebeConstruirRespuestaSinConsultarDetalle()
    {
        var command = CreateCommand();
        var detail = CreateDetail(command);
        _repository.ExistsVacunoAsync(command.VacunoReceptorId, Arg.Any<CancellationToken>())
            .Returns(true);
        _repository.HasActiveFecundacionAsync(
                command.Id,
                command.VacunoReceptorId,
                Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.UpdateAsync(
                command.Id,
                Arg.Any<FecundacionUpdateValues>(),
                Arg.Any<CancellationToken>())
            .Returns(CreateUpdateData(command, detail));

        var result = await _sut.HandleAsync(command);

        result.Id.Should().Be(command.Id);
        result.VacunoReceptorCodigo.Should().Be(detail.VacunoReceptorCodigo);
        await _repository.DidNotReceive().GetForEditAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_DebeLanzarNotFoundSinConsultarDetalle_CuandoUpdateNoEncuentraRegistro()
    {
        var command = CreateCommand();
        _repository.ExistsVacunoAsync(command.VacunoReceptorId, Arg.Any<CancellationToken>())
            .Returns(true);
        _repository.HasActiveFecundacionAsync(
                command.Id,
                command.VacunoReceptorId,
                Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.UpdateAsync(
                command.Id,
                Arg.Any<FecundacionUpdateValues>(),
                Arg.Any<CancellationToken>())
            .Returns((FecundacionUpdateData?)null);

        var action = () => _sut.HandleAsync(command);

        await action.Should().ThrowAsync<FecundacionNotFoundException>();
        await _repository.DidNotReceive().GetForEditAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
        await _cache.DidNotReceive().RemoveByPrefixAsync(Arg.Any<string>());
    }

    private static UpdateFecundacionCommand CreateCommand()
        => new(
            Id: 6036,
            TipoFecundacionCode: "INSEMINACION_ARTIFICIAL",
            VacunoReceptorId: 10,
            TipoDonante: "INTERNO",
            VacunoDonanteId: 20,
            ExternoDonanteNombre: null,
            FechaProcedimiento: new DateOnly(2026, 7, 20),
            ResponsableNombre: "Veterinario",
            ResultadoCode: "PENDIENTE",
            EstadoFecundacionCode: "EN_ESPERA",
            ObservacionesVeterinarias: "Sin observaciones",
            CodigoSemen: "SEM-01",
            CodigoEmbrion: null);

    private static FecundacionEditData CreateDetail(UpdateFecundacionCommand command)
        => new(
            Id: command.Id,
            Codigo: "FEC-6036",
            TipoFecundacionCode: command.TipoFecundacionCode,
            VacunoReceptorId: command.VacunoReceptorId,
            VacunoReceptorCodigo: "VAC-010",
            VacunoReceptorNombre: "Luna",
            TipoDonante: command.TipoDonante,
            VacunoDonanteId: command.VacunoDonanteId,
            VacunoDonanteCodigo: "VAC-020",
            VacunoDonanteNombre: "Toro",
            ExternoDonanteId: null,
            ExternoDonanteNombre: null,
            FechaProcedimiento: command.FechaProcedimiento,
            ResponsableNombre: command.ResponsableNombre,
            ResultadoCode: command.ResultadoCode,
            EstadoFecundacionCode: command.EstadoFecundacionCode,
            ObservacionesVeterinarias: command.ObservacionesVeterinarias,
            CodigoSemen: command.CodigoSemen,
            CodigoEmbrion: command.CodigoEmbrion,
            CreadoEn: new DateTime(2026, 7, 20),
            ActualizadoEn: new DateTime(2026, 7, 21));

    private static FecundacionUpdateData CreateUpdateData(
        UpdateFecundacionCommand command,
        FecundacionEditData detail)
        => new(
            detail.Id,
            detail.Codigo,
            detail.TipoFecundacionCode,
            detail.VacunoReceptorId,
            detail.VacunoReceptorCodigo,
            detail.VacunoReceptorNombre,
            detail.TipoDonante,
            detail.VacunoDonanteId,
            detail.VacunoDonanteCodigo,
            detail.VacunoDonanteNombre,
            detail.ExternoDonanteNombre,
            detail.FechaProcedimiento,
            detail.ResponsableNombre,
            command.ResultadoCode,
            command.EstadoFecundacionCode,
            detail.ObservacionesVeterinarias,
            detail.CodigoSemen,
            detail.CodigoEmbrion,
            detail.ActualizadoEn,
            null);
}
