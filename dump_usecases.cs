
=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/DeleteFecundacion/DeleteFecundacionCommand.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public sealed record DeleteFecundacionCommand(string Razon);

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/DeleteFecundacion/DeleteFecundacionInteractor.cs ===
using System;
using System.Threading;
using System.Threading.Tasks;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public sealed class DeleteFecundacionInteractor : IDeleteFecundacionInputPort
{
    private readonly IFecundacionRepository _repository;

    public DeleteFecundacionInteractor(IFecundacionRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(long id, DeleteFecundacionCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Razon))
        {
            throw new ArgumentException("Falta raz├│n de eliminaci├│n o datos inv├ílidos.");
        }

        var existing = await _repository.GetForEditAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new FecundacionNotFoundException();
        }

        // Validar si tiene cr├¡as vinculadas en trazabilidad
        var hasCria = await _repository.HasCriaAsync(id, cancellationToken);
        if (hasCria)
        {
            throw new FecundacionHasDependenciesException();
        }

        await _repository.DeleteAsync(id, command.Razon, cancellationToken);
    }
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/DeleteFecundacion/IDeleteFecundacionInputPort.cs ===
using System.Threading;
using System.Threading.Tasks;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.DeleteFecundacion;

public interface IDeleteFecundacionInputPort
{
    Task HandleAsync(long id, DeleteFecundacionCommand command, CancellationToken cancellationToken);
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/GetFecundacionForEdit/GetFecundacionForEditInteractor.cs ===
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

public sealed class GetFecundacionForEditInteractor : IGetFecundacionForEditInputPort
{
    private readonly IFecundacionRepository _repository;

    public GetFecundacionForEditInteractor(IFecundacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetFecundacionForEditOutput> HandleAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var data = await _repository.GetForEditAsync(id, cancellationToken)
            ?? throw new NotFoundException($"No se encontr├│ la fecundaci├│n con ID {id}.");

        return new GetFecundacionForEditOutput(
            data.Id,
            data.Codigo,
            data.TipoFecundacionCode,
            data.VacunoReceptorId,
            data.VacunoReceptorCodigo,
            data.VacunoReceptorNombre,
            data.TipoDonante,
            data.VacunoDonanteId,
            data.VacunoDonanteCodigo,
            data.VacunoDonanteNombre,
            data.ExternoDonanteId,
            data.ExternoDonanteNombre,
            data.FechaProcedimiento,
            data.ResponsableNombre,
            data.ResultadoCode,
            data.EstadoFecundacionCode,
            data.ObservacionesVeterinarias,
            data.CodigoSemen,
            data.CodigoEmbrion,
            data.CreadoEn,
            data.ActualizadoEn);
    }
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/GetFecundacionForEdit/GetFecundacionForEditOutput.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

public sealed record GetFecundacionForEditOutput(
    long Id,
    string Codigo,
    string TipoFecundacionCode,
    long VacunoReceptorId,
    string VacunoReceptorCodigo,
    string VacunoReceptorNombre,
    string TipoDonante,
    long? VacunoDonanteId,
    string? VacunoDonanteCodigo,
    string? VacunoDonanteNombre,
    long? ExternoDonanteId,
    string? ExternoDonanteNombre,
    DateOnly FechaProcedimiento,
    string ResponsableNombre,
    string ResultadoCode,
    string EstadoFecundacionCode,
    string? ObservacionesVeterinarias,
    string? CodigoSemen,
    string? CodigoEmbrion,
    DateTime CreadoEn,
    DateTime ActualizadoEn);

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/GetFecundacionForEdit/IGetFecundacionForEditInputPort.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionForEdit;

public interface IGetFecundacionForEditInputPort
{
    Task<GetFecundacionForEditOutput> HandleAsync(long id, CancellationToken cancellationToken = default);
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/GetFecundacionOptions/GetFecundacionOptionsInteractor.cs ===
using ZooTech.Domain.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;

public sealed class GetFecundacionOptionsInteractor : IGetFecundacionOptionsInputPort
{
    private readonly IFecundacionRepository _repository;

    public GetFecundacionOptionsInteractor(IFecundacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetFecundacionOptionsOutput> HandleAsync(CancellationToken cancellationToken = default)
    {
        var data = await _repository.GetOptionsAsync(cancellationToken);

        return new GetFecundacionOptionsOutput(
            data.Tipos.Select(ToOutput).ToList(),
            data.Resultados.Select(ToOutput).ToList(),
            data.Estados.Select(ToOutput).ToList());
    }

    private static FecundacionOptionOutput ToOutput(FecundacionOptionData option)
        => new(option.Code, option.Nombre, option.Descripcion);
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/GetFecundacionOptions/GetFecundacionOptionsOutput.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;

public sealed record FecundacionOptionOutput(string Code, string Nombre, string? Descripcion);

public sealed record GetFecundacionOptionsOutput(
    IReadOnlyList<FecundacionOptionOutput> Tipos,
    IReadOnlyList<FecundacionOptionOutput> Resultados,
    IReadOnlyList<FecundacionOptionOutput> Estados);

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/GetFecundacionOptions/IGetFecundacionOptionsInputPort.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.GetFecundacionOptions;

public interface IGetFecundacionOptionsInputPort
{
    Task<GetFecundacionOptionsOutput> HandleAsync(CancellationToken cancellationToken = default);
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/ListarFecundacionesPaginado/IListarFecundacionesPaginadoInputPort.cs ===
using System.Threading;
using System.Threading.Tasks;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacionesPaginado;

public interface IListarFecundacionesPaginadoInputPort
{
    Task<ListarFecundacionesPaginadoOutput> HandleAsync(
        ListarFecundacionesPaginadoCommand command,
        CancellationToken cancellationToken);
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/ListarFecundacionesPaginado/ListarFecundacionesPaginadoCommand.cs ===
using System;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacionesPaginado;

public sealed record ListarFecundacionesPaginadoCommand(
    int Page,
    int Limit,
    DateOnly? FechaDesde,
    DateOnly? FechaHasta,
    string? Q,
    string? TipoFecundacion,
    string? Estado,
    string? Responsable);

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/ListarFecundacionesPaginado/ListarFecundacionesPaginadoInteractor.cs ===
using System;
using System.Threading;
using System.Threading.Tasks;
using ZooTech.Application.Modules.Module_Fecundacion.Common;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacionesPaginado;

public sealed class ListarFecundacionesPaginadoInteractor : IListarFecundacionesPaginadoInputPort
{
    private readonly IFecundacionQueryRepository _queryRepository;

    public ListarFecundacionesPaginadoInteractor(IFecundacionQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<ListarFecundacionesPaginadoOutput> HandleAsync(
        ListarFecundacionesPaginadoCommand command,
        CancellationToken cancellationToken)
    {
        var (data, total) = await _queryRepository.GetPagedAsync(
            command.Page,
            command.Limit,
            command.FechaDesde,
            command.FechaHasta,
            command.Q,
            command.TipoFecundacion,
            command.Estado,
            command.Responsable,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)total / command.Limit);

        return new ListarFecundacionesPaginadoOutput(
            data,
            command.Page,
            command.Limit,
            total,
            totalPages == 0 ? 1 : totalPages);
    }
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/ListarFecundacionesPaginado/ListarFecundacionesPaginadoOutput.cs ===
using System.Collections.Generic;
using ZooTech.Application.Modules.Module_Fecundacion.Common;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.ListarFecundacionesPaginado;

public sealed record ListarFecundacionesPaginadoOutput(
    List<FecundacionItemDto> Data,
    int Page,
    int Limit,
    int Total,
    int TotalPages);

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/SearchFecundacionVacunos/ISearchFecundacionVacunosInputPort.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;

public interface ISearchFecundacionVacunosInputPort
{
    Task<IReadOnlyList<SearchFecundacionVacunoOutput>> HandleAsync(
        SearchFecundacionVacunosQuery query,
        CancellationToken cancellationToken = default);
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/SearchFecundacionVacunos/SearchFecundacionVacunoOutput.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;

public sealed record SearchFecundacionVacunoOutput(long Id, string Codigo, string Nombre, string Sexo);

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/SearchFecundacionVacunos/SearchFecundacionVacunosInteractor.cs ===
using ZooTech.Domain.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;

public sealed class SearchFecundacionVacunosInteractor : ISearchFecundacionVacunosInputPort
{
    private readonly IFecundacionRepository _repository;

    public SearchFecundacionVacunosInteractor(IFecundacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SearchFecundacionVacunoOutput>> HandleAsync(
        SearchFecundacionVacunosQuery query,
        CancellationToken cancellationToken = default)
    {
        var data = await _repository.SearchVacunosAsync(query.Sexo, query.Query, cancellationToken);

        return data
            .Select(item => new SearchFecundacionVacunoOutput(item.Id, item.Codigo, item.Nombre, item.Sexo))
            .ToList();
    }
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/SearchFecundacionVacunos/SearchFecundacionVacunosQuery.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;

public sealed record SearchFecundacionVacunosQuery(string? Sexo, string? Query);

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/UpdateFecundacion/IUpdateFecundacionInputPort.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public interface IUpdateFecundacionInputPort
{
    Task<UpdateFecundacionOutput> HandleAsync(
        UpdateFecundacionCommand command,
        CancellationToken cancellationToken = default);
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/UpdateFecundacion/UpdateFecundacionCommand.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public sealed record UpdateFecundacionCommand(
    long Id,
    string TipoFecundacionCode,
    long VacunoReceptorId,
    string TipoDonante,
    long? VacunoDonanteId,
    string? ExternoDonanteNombre,
    DateOnly FechaProcedimiento,
    string ResponsableNombre,
    string ResultadoCode,
    string EstadoFecundacionCode,
    string? ObservacionesVeterinarias,
    string? CodigoSemen,
    string? CodigoEmbrion);

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/UpdateFecundacion/UpdateFecundacionInteractor.cs ===
using FluentValidation;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.Domain.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public sealed class UpdateFecundacionInteractor : IUpdateFecundacionInputPort
{
    private readonly IFecundacionRepository _repository;
    private readonly IValidator<UpdateFecundacionCommand> _validator;

    public UpdateFecundacionInteractor(
        IFecundacionRepository repository,
        IValidator<UpdateFecundacionCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<UpdateFecundacionOutput> HandleAsync(
        UpdateFecundacionCommand command,
        CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var existing = await _repository.GetForEditAsync(command.Id, cancellationToken);
        if (existing is null)
            throw new FecundacionNotFoundException();

        if (!await _repository.ExistsVacunoAsync(command.VacunoReceptorId, cancellationToken))
            throw new FecundacionVacunoNotFoundException();

        if (await _repository.HasActiveFecundacionAsync(command.Id, command.VacunoReceptorId, cancellationToken))
            throw new FecundacionPendingActiveException();

        var values = new FecundacionUpdateValues(
            command.TipoFecundacionCode,
            command.VacunoReceptorId,
            command.TipoDonante,
            command.VacunoDonanteId,
            command.ExternoDonanteNombre,
            command.FechaProcedimiento,
            command.ResponsableNombre,
            command.ResultadoCode,
            command.EstadoFecundacionCode,
            command.ObservacionesVeterinarias,
            command.CodigoSemen,
            command.CodigoEmbrion);

        var updated = await _repository.UpdateAsync(command.Id, values, cancellationToken)
            ?? throw new FecundacionNotFoundException();

        var detail = await _repository.GetForEditAsync(command.Id, cancellationToken)
            ?? throw new FecundacionNotFoundException();

        return new UpdateFecundacionOutput(
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
            updated.ResultadoCode,
            updated.EstadoFecundacionCode,
            detail.ObservacionesVeterinarias,
            detail.CodigoSemen,
            detail.CodigoEmbrion,
            detail.ActualizadoEn,
            updated.Warning);
    }
}

=== src/ZooTech.Application/Modules/Module_Fecundacion/UseCases/UpdateFecundacion/UpdateFecundacionOutput.cs ===
namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;

public sealed record UpdateFecundacionOutput(
    long Id,
    string Codigo,
    string TipoFecundacionCode,
    long VacunoReceptorId,
    string VacunoReceptorCodigo,
    string VacunoReceptorNombre,
    string TipoDonante,
    long? VacunoDonanteId,
    string? VacunoDonanteCodigo,
    string? VacunoDonanteNombre,
    string? ExternoDonanteNombre,
    DateOnly FechaProcedimiento,
    string ResponsableNombre,
    string ResultadoCode,
    string EstadoFecundacionCode,
    string? ObservacionesVeterinarias,
    string? CodigoSemen,
    string? CodigoEmbrion,
    DateTime ActualizadoEn,
    string? Warning);

=== src/ZooTech.Application/Modules/Module_Fecundacion/Validators/UpdateFecundacionValidator.cs ===
using FluentValidation;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Domain.Module_Fecundacion.Rules;

namespace ZooTech.Application.Modules.Module_Fecundacion.Validators;

public sealed class UpdateFecundacionValidator : AbstractValidator<UpdateFecundacionCommand>
{
    public UpdateFecundacionValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.TipoFecundacionCode).NotEmpty().MaximumLength(40);
        RuleFor(command => command.VacunoReceptorId).GreaterThan(0);
        RuleFor(command => command.FechaProcedimiento)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha del procedimiento no puede ser futura.");
        RuleFor(command => command.ResponsableNombre).NotEmpty().MaximumLength(100);
        RuleFor(command => command.ResultadoCode).NotEmpty().MaximumLength(30);
        RuleFor(command => command.EstadoFecundacionCode).NotEmpty().MaximumLength(30);
        RuleFor(command => command.ObservacionesVeterinarias).MaximumLength(250);

        RuleFor(command => command.TipoDonante)
            .NotEmpty()
            .Must(tipo => IsDonante(tipo, FecundacionRules.TipoDonanteInterno) || IsDonante(tipo, FecundacionRules.TipoDonanteExterno))
            .WithMessage("El tipo de donante debe ser INTERNO o EXTERNO.");

        When(command => IsDonante(command.TipoDonante, FecundacionRules.TipoDonanteInterno), () =>
        {
            RuleFor(command => command.VacunoDonanteId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un donante interno.");
        });

        When(command => IsDonante(command.TipoDonante, FecundacionRules.TipoDonanteExterno), () =>
        {
            RuleFor(command => command.ExternoDonanteNombre)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Debe ingresar el nombre del donante externo.");
        });

        When(command => FecundacionRules.EsInseminacionArtificial(command.TipoFecundacionCode), () =>
        {
            RuleFor(command => command.CodigoSemen)
                .NotEmpty()
                .MaximumLength(30)
                .WithMessage("El c├│digo de semen es obligatorio para inseminaci├│n artificial.");
        });

        When(command => FecundacionRules.EsTransferenciaEmbriones(command.TipoFecundacionCode), () =>
        {
            RuleFor(command => command.CodigoEmbrion)
                .NotEmpty()
                .MaximumLength(30)
                .WithMessage("El c├│digo de embri├│n es obligatorio para transferencia de embriones.");
        });
    }

    private static bool IsDonante(string? value, string expected)
        => string.Equals(value, expected, StringComparison.OrdinalIgnoreCase);
}

=== src/ZooTech.InterfaceAdapters/Modules/Module_Fecundacion/DTOs/Responses/FecundacionEditResponse.cs ===
using System.Text.Json.Serialization;

namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

public sealed record FecundacionEditResponse(
    long Id,
    string Codigo,
    string TipoFecundacion,
    VacunoResumenResponse VacunoReceptor,
    object MachoODonante,
    bool MachoExterno,
    DateOnly FechaProcedimiento,
    string Responsable,
    string Resultado,
    string? CodigoSemen,
    string? CodigoEmbrion,
    string? Observaciones,
    string EstadoFecundacion,
    DateTime CreadoEn,
    DateTime ActualizadoEn);

public sealed record VacunoResumenResponse(long Id, string Codigo, string Nombre);

=== src/ZooTech.InterfaceAdapters/Modules/Module_Fecundacion/DTOs/Responses/FecundacionOptionsResponse.cs ===
namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

public sealed record FecundacionOptionResponse(string Code, string Nombre, string? Descripcion);

public sealed record FecundacionOptionsResponse(
    IReadOnlyList<FecundacionOptionResponse> Tipos,
    IReadOnlyList<FecundacionOptionResponse> Resultados,
    IReadOnlyList<FecundacionOptionResponse> Estados);

=== src/ZooTech.InterfaceAdapters/Modules/Module_Fecundacion/DTOs/Responses/FecundacionUpdateResponse.cs ===
namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

public sealed record FecundacionUpdateResponse(
    long Id,
    string TipoFecundacion,
    VacunoResumenResponse VacunoReceptor,
    object MachoODonante,
    DateOnly FechaProcedimiento,
    string Responsable,
    string Resultado,
    string? CodigoSemen,
    string? CodigoEmbrion,
    string? Observaciones,
    DateTime ActualizadoEn,
    string? Warning);

=== src/ZooTech.InterfaceAdapters/Modules/Module_Fecundacion/DTOs/Responses/FecundacionVacunoOptionResponse.cs ===
namespace ZooTech.InterfaceAdapters.Modules.Module_Fecundacion.DTOs.Responses;

public sealed record FecundacionVacunoOptionResponse(
    long Id,
    string Codigo,
    string Nombre,
    string Sexo);

=== tests/Unit/ZooTech.Application.UnitTests/Modules/Module_Fecundacion/Validators/UpdateFecundacionValidatorTests.cs ===
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.UpdateFecundacion;
using ZooTech.Application.Modules.Module_Fecundacion.Validators;

namespace ZooTech.Application.UnitTests.Modules.Module_Fecundacion.Validators;

public sealed class UpdateFecundacionValidatorTests
{
    private readonly UpdateFecundacionValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenFechaProcedimientoIsFuture()
    {
        var command = BuildCommand(fechaProcedimiento: DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFecundacionCommand.FechaProcedimiento));
    }

    [Fact]
    public void Validate_ShouldFail_WhenIaDoesNotHaveCodigoSemen()
    {
        var command = BuildCommand(tipoFecundacionCode: "IA", codigoSemen: null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFecundacionCommand.CodigoSemen));
    }

    [Fact]
    public void Validate_ShouldFail_WhenTeDoesNotHaveCodigoEmbrion()
    {
        var command = BuildCommand(tipoFecundacionCode: "TE", codigoEmbrion: null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFecundacionCommand.CodigoEmbrion));
    }

    [Fact]
    public void Validate_ShouldPass_WhenMontaNaturalHasInternalDonor()
    {
        var command = BuildCommand(tipoFecundacionCode: "MN");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    private static UpdateFecundacionCommand BuildCommand(
        string tipoFecundacionCode = "MN",
        DateOnly? fechaProcedimiento = null,
        string? codigoSemen = "SEM-001",
        string? codigoEmbrion = "EMB-001")
        => new(
            Id: 1,
            TipoFecundacionCode: tipoFecundacionCode,
            VacunoReceptorId: 2,
            TipoDonante: "INTERNO",
            VacunoDonanteId: 3,
            ExternoDonanteNombre: null,
            FechaProcedimiento: fechaProcedimiento ?? DateOnly.FromDateTime(DateTime.Today),
            ResponsableNombre: "Tec. Ruiz",
            ResultadoCode: "PENDIENTE",
            EstadoFecundacionCode: "PENDIENTE",
            ObservacionesVeterinarias: null,
            CodigoSemen: codigoSemen,
            CodigoEmbrion: codigoEmbrion);
}

