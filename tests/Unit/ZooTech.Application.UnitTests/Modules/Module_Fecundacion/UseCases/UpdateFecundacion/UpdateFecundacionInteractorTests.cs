using FluentAssertions;
using NSubstitute;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Caching;
using ZooTech.Application.Modules.Module_Fecundacion.Common;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Domain.Ganaderia.Module_Fecundacion.Interfaces;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Interfaces;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public sealed class UpdateFecundacionInteractorTests
{
    private readonly IFecundacionRepository _repository = Substitute.For<IFecundacionRepository>();
    private readonly IAppCacheService _cache = Substitute.For<IAppCacheService>();
    private readonly IGanaderiaUnitOfWork _unitOfWork = Substitute.For<IGanaderiaUnitOfWork>();
    private readonly IFecundacionObservationPolicy _observationPolicy =
        Substitute.For<IFecundacionObservationPolicy>();
    private readonly UpdateFecundacionInteractor _sut;

    public UpdateFecundacionInteractorTests()
    {
        _unitOfWork.Fecundaciones.Returns(_repository);
        _sut = new UpdateFecundacionInteractor(_unitOfWork, _cache, _observationPolicy);
    }

    [Fact]
    public async Task HandleAsync_NoDebeConsultarNiActualizar_CuandoObservacionesExcedenLimiteTenant()
    {
        var command = CreateValidCommand();
        _observationPolicy.ValidateAsync(
                command.ObservacionesVeterinarias,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new ValidationException(
                ["FECUNDACION-OBSERVACIONES_VETERINARIAS-MAX_LENGTH"],
                ScopeName.Application)));

        var action = () => _sut.HandleAsync(command);

        await action.Should().ThrowAsync<ValidationException>();
        await _repository.DidNotReceive().GetForEditAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().UpdateAsync(
            Arg.Any<long>(),
            Arg.Any<FecundacionUpdateValues>(),
            Arg.Any<CancellationToken>());
        await _cache.DidNotReceive().RemoveByPrefixAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task HandleAsync_DebeValidarObservacionesYActualizar_CuandoDatosSonValidos()
    {
        var command = CreateValidCommand();
        var detail = CreateEditData(command);
        _repository.GetForEditAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(detail, detail);
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
            .Returns(new FecundacionUpdateData(
                command.Id,
                detail.Codigo,
                command.ResultadoCode,
                command.EstadoFecundacionCode,
                null));

        var result = await _sut.HandleAsync(command);

        result.Id.Should().Be(command.Id);
        await _observationPolicy.Received(1).ValidateAsync(
            command.ObservacionesVeterinarias,
            Arg.Any<CancellationToken>());
        await _repository.Received(1).UpdateAsync(
            command.Id,
            Arg.Is<FecundacionUpdateValues>(values =>
                values.ObservacionesVeterinarias == command.ObservacionesVeterinarias),
            Arg.Any<CancellationToken>());
        await _cache.Received(1).RemoveByPrefixAsync("fecundacion:listar");
    }

    private static UpdateFecundacionCommand CreateValidCommand()
        => new(
            Id: 10,
            TipoFecundacionCode: "IA",
            VacunoReceptorId: 20,
            TipoDonante: "INTERNO",
            VacunoDonanteId: 30,
            ExternoDonanteNombre: null,
            FechaProcedimiento: new DateOnly(2026, 7, 20),
            ResponsableNombre: "Veterinario",
            ResultadoCode: "PENDIENTE",
            EstadoFecundacionCode: "ACTIVA",
            ObservacionesVeterinarias: "Sin incidencias",
            CodigoSemen: "SEM-01",
            CodigoEmbrion: null);

    private static FecundacionEditData CreateEditData(UpdateFecundacionCommand command)
        => new(
            Id: command.Id,
            Codigo: "FEC-010",
            TipoFecundacionCode: command.TipoFecundacionCode,
            VacunoReceptorId: command.VacunoReceptorId,
            VacunoReceptorCodigo: "VAC-020",
            VacunoReceptorNombre: "Luna",
            TipoDonante: command.TipoDonante,
            VacunoDonanteId: command.VacunoDonanteId,
            VacunoDonanteCodigo: "VAC-030",
            VacunoDonanteNombre: "Toro",
            ExternoDonanteId: null,
            ExternoDonanteNombre: command.ExternoDonanteNombre,
            FechaProcedimiento: command.FechaProcedimiento,
            ResponsableNombre: command.ResponsableNombre,
            ResultadoCode: command.ResultadoCode,
            EstadoFecundacionCode: command.EstadoFecundacionCode,
            ObservacionesVeterinarias: command.ObservacionesVeterinarias,
            CodigoSemen: command.CodigoSemen,
            CodigoEmbrion: command.CodigoEmbrion,
            CreadoEn: new DateTime(2026, 7, 20),
            ActualizadoEn: new DateTime(2026, 7, 21));
}
