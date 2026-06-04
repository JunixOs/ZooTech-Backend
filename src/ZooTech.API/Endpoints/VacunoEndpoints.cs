using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZooTech.API.Configuration;
using ZooTech.API.Models;
using ZooTech.API.Security;
using ZooTech.API.Services;

namespace ZooTech.API.Endpoints;

public static class VacunoEndpoints
{
    public static RouteGroupBuilder MapVacunoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/vacunos")
            .WithTags("Vacunos")
            .AddEndpointFilter<DemoAuthEndpointFilter>();

        group.MapGet("/", (
                string? search,
                int? page,
                int? pageSize,
                IVacunoRepository repository) =>
            {
                var response = repository.Search(search, page ?? 1, pageSize ?? 10);
                return Results.Ok(response);
            })
            .WithName("ListarVacunos")
            .WithSummary("Lista vacunos paginados para la vista principal.");

        group.MapPost("/", async (
                HttpRequest httpRequest,
                IVacunoRepository repository,
                CancellationToken cancellationToken) =>
            {
                var parsed = await TryReadRegistrarRequest(httpRequest, cancellationToken);

                if (parsed.Error is not null)
                {
                    return Results.BadRequest(parsed.Error);
                }

                var request = parsed.Request!;
                var validationErrors = ValidateRegistrarRequest(request);

                if (validationErrors.Count > 0)
                {
                    return Results.BadRequest(new ApiErrorEnvelope(new ApiError(
                        Code: "VALIDATION_ERROR",
                        Message: "La solicitud contiene datos invalidos.",
                        Details: validationErrors)));
                }

                if (repository.GetByCodigo(request.Codigo) is not null)
                {
                    return Results.Conflict(new ApiErrorEnvelope(new ApiError(
                        Code: "VACUNO_ALREADY_EXISTS",
                        Message: $"Ya existe un vacuno con el codigo {request.Codigo}.",
                        Details: Array.Empty<ApiValidationError>())));
                }

                try
                {
                    var created = repository.Create(request);
                    var response = new RegistrarVacunoResponse(
                        Codigo: created.Codigo,
                        Nombre: created.Nombre,
                        FechaNacimiento: request.FechaNacimiento,
                        AdquisicionPor: request.AdquisicionPor,
                        PrecioCompra: request.PrecioCompra,
                        FotoUrl: string.IsNullOrWhiteSpace(created.FotoUrl) ? null : created.FotoUrl,
                        CreadoEn: DateTime.UtcNow);

                    return Results.Created($"/api/vacunos/{created.Codigo}", response);
                }
                catch (InvalidOperationException ex) when (ex.Message == "VACUNO_ALREADY_EXISTS")
                {
                    return Results.Conflict(new ApiErrorEnvelope(new ApiError(
                        Code: "VACUNO_ALREADY_EXISTS",
                        Message: $"Ya existe un vacuno con el codigo {request.Codigo}.",
                        Details: Array.Empty<ApiValidationError>())));
                }
            })
            .WithName("RegistrarVacuno")
            .WithSummary("Registra un nuevo vacuno desde el formulario.");

        group.MapGet("/estadisticas/actividad", (
                DateOnly? fechaInicio,
                DateOnly? fechaFin,
                IVacunoRepository repository,
                IOptions<VacunoRequirementOptions> options) =>
            {
                var reglas = options.Value.GraficoVacunosEnActividad;
                var diasPorDefecto = Math.Max(1, reglas.DiasPorDefecto);
                var maximoDiasRango = Math.Max(1, reglas.MaximoDiasRango);
                var today = DateOnly.FromDateTime(DateTime.Today);
                var start = fechaInicio ?? today.AddDays(-(diasPorDefecto - 1));
                var end = fechaFin ?? today;

                if (start > end)
                {
                    return Results.BadRequest(new
                    {
                        message = "La fecha inicio no puede ser mayor que la fecha fin."
                    });
                }

                var diasSolicitados = end.DayNumber - start.DayNumber + 1;
                if (diasSolicitados > maximoDiasRango)
                {
                    return Results.BadRequest(new
                    {
                        message = $"El rango de fechas no puede superar {maximoDiasRango} dias."
                    });
                }

                return Results.Ok(repository.GetActivityStats(
                    start,
                    end,
                    new VacunoActividadParametros(reglas.ContarEliminadosHastaFechaEliminacion)));
            })
            .WithName("EstadisticasVacunosEnActividad")
            .WithSummary("Devuelve la serie historica de vacunos en actividad para el grafico de reportes.");

        group.MapGet("/{codigoOId}/genealogia", (
                string codigoOId,
                int? niveles,
                IVacunoRepository repository) =>
            {
                var arbol = repository.GetGenealogia(codigoOId, niveles ?? 4);

                return arbol is null
                    ? Results.NotFound(new ApiErrorEnvelope(new ApiError(
                        Code: "VACUNO_NOT_FOUND",
                        Message: $"No se encontro el vacuno {codigoOId} o fue eliminado.",
                        Details: Array.Empty<ApiValidationError>())))
                    : Results.Ok(arbol);
            })
            .WithName("GraficoGenealogicoVacuno")
            .WithSummary("Devuelve el arbol genealogico de un vacuno hasta 4 niveles.");

        group.MapGet("/{codigo}", (
                string codigo,
                IVacunoRepository repository,
                IOptions<VacunoRequirementOptions> options) =>
            {
                var reglas = options.Value.VerVacuno;
                var vacuno = repository.GetByCodigo(
                    codigo,
                    new VerVacunoParametros(
                        reglas.ExcluirEliminados,
                        reglas.PermitirBusquedaCodigoSinSeparadores));

                return vacuno is null ? Results.NotFound() : Results.Ok(vacuno);
            })
            .WithName("VerVacuno")
            .WithSummary("Obtiene el detalle completo de un vacuno por codigo.");

        group.MapPut("/{codigo}", (
                string codigo,
                [FromBody] UpdateVacunoRequest request,
                IVacunoRepository repository) =>
            {
                var validationErrors = ValidateUpdateRequest(request);

                if (validationErrors.Count > 0)
                {
                    return Results.BadRequest(new ApiErrorEnvelope(new ApiError(
                        Code: "VACUNO_UPDATE_VALIDATION_ERROR",
                        Message: "La solicitud contiene datos invalidos.",
                        Details: validationErrors)));
                }

                try
                {
                    var updated = repository.Update(codigo, request);
                    return updated is null ? Results.NotFound() : Results.Ok(updated);
                }
                catch (InvalidOperationException ex) when (ex.Message == "GRANJA_NOT_FOUND")
                {
                    return Results.BadRequest(new ApiErrorEnvelope(new ApiError(
                        Code: "VACUNO_UPDATE_VALIDATION_ERROR",
                        Message: "La granja indicada no existe.",
                        Details:
                        [
                            new ApiValidationError("Granja", "La granja indicada no existe.")
                        ])));
                }
            })
            .WithName("EditarVacuno")
            .WithSummary("Actualiza los datos visibles en la pantalla Ver Vacuno.");

        group.MapDelete("/{codigo}", (
                string codigo,
                [FromBody] DeleteVacunoRequest request,
                IVacunoRepository repository) =>
            {
                var validationErrors = ValidateDeleteRequest(request);

                if (validationErrors.Count > 0)
                {
                    return Results.BadRequest(new ApiErrorEnvelope(new ApiError(
                        Code: "VACUNO_DELETE_VALIDATION_ERROR",
                        Message: "La solicitud contiene datos invalidos.",
                        Details: validationErrors)));
                }

                var deleted = repository.Delete(codigo, request.MotivoEliminacion!.Trim());

                return deleted is null ? Results.NotFound() : Results.Ok(deleted);
            })
            .WithName("EliminarVacuno")
            .WithSummary("Elimina logicamente un vacuno de la lista.");

        group.MapGet("/options", (IVacunoRepository repository) => Results.Ok(repository.GetOptions()))
            .WithName("OpcionesVacuno")
            .WithSummary("Devuelve valores usados por los combos de la vista.");

        return group;
    }

    private static async Task<(RegistrarVacunoRequest? Request, ApiErrorEnvelope? Error)> TryReadRegistrarRequest(
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        if (httpRequest.HasFormContentType)
        {
            var form = await httpRequest.ReadFormAsync(cancellationToken);

            return (new RegistrarVacunoRequest(
                Codigo: GetFormValue(form, "codigo"),
                Nombre: GetFormValue(form, "nombre"),
                FechaNacimiento: ParseDate(GetFormValue(form, "fechaNacimiento")),
                AdquisicionPor: GetFormValue(form, "adquisicionPor"),
                PrecioCompra: ParseDecimal(GetFormValue(form, "precioCompra", required: false)),
                Raza: GetFormValue(form, "raza"),
                Color: GetFormValue(form, "color"),
                Sexo: GetFormValue(form, "sexo"),
                CodigoPadre: GetFormValue(form, "codigoPadre"),
                CodigoMadre: GetFormValue(form, "codigoMadre"),
                Granja: GetFormValue(form, "granja"),
                Distrito: GetFormValue(form, "distrito"),
                Departamento: GetFormValue(form, "departamento"),
                Provincia: GetFormValue(form, "provincia"),
                AptoPara: GetFormValue(form, "aptoPara"),
                FechaEspecificacion: ParseDate(GetFormValue(form, "fechaEspecificacion")),
                Observaciones: GetFormValue(form, "observaciones", required: false),
                FotoUrl: null,
                CodigoDistrito: GetFormValue(form, "codigoDistrito", required: false),
                CodigoDepartamento: GetFormValue(form, "codigoDepartamento", required: false),
                CodigoProvincia: GetFormValue(form, "codigoProvincia", required: false)), null);
        }

        var request = await httpRequest.ReadFromJsonAsync<RegistrarVacunoRequest>(
            cancellationToken: cancellationToken);

        return request is null
            ? (null, new ApiErrorEnvelope(new ApiError(
                Code: "INVALID_BODY",
                Message: "El cuerpo de la solicitud no es valido.",
                Details: Array.Empty<ApiValidationError>())))
            : (request, null);
    }

    private static string GetFormValue(IFormCollection form, string key, bool required = true)
    {
        var value = form[key].ToString();

        if (required && string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Trim();
    }

    private static DateOnly ParseDate(string value)
    {
        return DateOnly.TryParse(value, out var date) ? date : default;
    }

    private static decimal? ParseDecimal(string value)
    {
        return decimal.TryParse(value, out var number) ? number : null;
    }

    private static List<ApiValidationError> ValidateRegistrarRequest(RegistrarVacunoRequest request)
    {
        var errors = new List<ApiValidationError>();

        Required(errors, "Codigo", request.Codigo, "El codigo es obligatorio.");
        Required(errors, "Nombre", request.Nombre, "El nombre es obligatorio.");
        Required(errors, "AdquisicionPor", request.AdquisicionPor, "La adquisicion es obligatoria.");
        Required(errors, "Raza", request.Raza, "La raza es obligatoria.");
        Required(errors, "Color", request.Color, "El color es obligatorio.");
        Required(errors, "Sexo", request.Sexo, "El sexo es obligatorio.");
        Required(errors, "CodigoPadre", request.CodigoPadre, "El codigo del padre es obligatorio.");
        Required(errors, "CodigoMadre", request.CodigoMadre, "El codigo de la madre es obligatorio.");
        Required(errors, "Granja", request.Granja, "La granja es obligatoria.");
        Required(errors, "Distrito", request.Distrito, "El distrito es obligatorio.");
        Required(errors, "Departamento", request.Departamento, "El departamento es obligatorio.");
        Required(errors, "Provincia", request.Provincia, "La provincia es obligatoria.");
        Required(errors, "AptoPara", request.AptoPara, "El campo apto para es obligatorio.");
        MaxLength(errors, "Distrito", request.Distrito, 60, "El distrito no puede exceder 60 caracteres.");
        MaxLength(errors, "Departamento", request.Departamento, 60, "El departamento no puede exceder 60 caracteres.");
        MaxLength(errors, "Provincia", request.Provincia, 60, "La provincia no puede exceder 60 caracteres.");

        if (request.FechaNacimiento == default)
        {
            errors.Add(new ApiValidationError("FechaNacimiento", "La fecha de nacimiento es obligatoria."));
        }
        else if (request.FechaNacimiento > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add(new ApiValidationError("FechaNacimiento", "La fecha de nacimiento no puede ser futura."));
        }

        if (request.FechaEspecificacion == default)
        {
            errors.Add(new ApiValidationError("FechaEspecificacion", "La fecha de registro es obligatoria."));
        }

        if (!string.IsNullOrWhiteSpace(request.Codigo) &&
            !System.Text.RegularExpressions.Regex.IsMatch(request.Codigo, "^[A-Z0-9]+$"))
        {
            errors.Add(new ApiValidationError("Codigo", "El codigo debe estar en mayusculas y solo puede contener letras y numeros."));
        }

        if (!string.Equals(request.AdquisicionPor, "compra", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(request.AdquisicionPor, "monta", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(new ApiValidationError("AdquisicionPor", "La adquisicion debe ser monta o compra."));
        }

        if (string.Equals(request.AdquisicionPor, "compra", StringComparison.OrdinalIgnoreCase) &&
            (request.PrecioCompra is null or <= 0))
        {
            errors.Add(new ApiValidationError("PrecioCompra", "El precio de compra debe ser mayor a 0."));
        }

        if (!string.IsNullOrWhiteSpace(request.Observaciones))
        {
            if (request.Observaciones.Length > 150)
            {
                errors.Add(new ApiValidationError("Observaciones", "Las observaciones no pueden exceder 150 caracteres."));
            }

            if (request.Observaciones.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > 30)
            {
                errors.Add(new ApiValidationError("Observaciones", "Las observaciones no pueden exceder 30 palabras."));
            }
        }

        return errors;
    }

    private static List<ApiValidationError> ValidateUpdateRequest(UpdateVacunoRequest request)
    {
        var errors = new List<ApiValidationError>();

        Required(errors, "Nombre", request.Nombre, "El nombre es obligatorio.");
        Required(errors, "AdquisicionPor", request.AdquisicionPor, "La adquisicion es obligatoria.");
        Required(errors, "Raza", request.Raza, "La raza es obligatoria.");
        Required(errors, "Color", request.Color, "El color es obligatorio.");
        Required(errors, "Sexo", request.Sexo, "El sexo es obligatorio.");
        Required(errors, "Granja", request.Granja, "La granja es obligatoria.");
        MaxLength(errors, "Distrito", request.Distrito, 60, "El distrito no puede exceder 60 caracteres.");
        MaxLength(errors, "Departamento", request.Departamento, 60, "El departamento no puede exceder 60 caracteres.");
        MaxLength(errors, "Provincia", request.Provincia, 60, "La provincia no puede exceder 60 caracteres.");

        if (!DateOnly.TryParse(request.FechaNacimiento, out var fechaNacimiento))
        {
            errors.Add(new ApiValidationError("FechaNacimiento", "La fecha de nacimiento es obligatoria."));
        }
        else if (fechaNacimiento > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add(new ApiValidationError("FechaNacimiento", "La fecha de nacimiento no puede ser futura."));
        }

        if (!string.IsNullOrWhiteSpace(request.FechaRegistroFuncion) &&
            !DateOnly.TryParse(request.FechaRegistroFuncion, out _))
        {
            errors.Add(new ApiValidationError("FechaRegistroFuncion", "La fecha de registro no tiene un formato valido."));
        }

        if (!string.IsNullOrWhiteSpace(request.Observaciones))
        {
            if (request.Observaciones.Length > 150)
            {
                errors.Add(new ApiValidationError("Observaciones", "Las observaciones no pueden exceder 150 caracteres."));
            }

            if (request.Observaciones.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > 30)
            {
                errors.Add(new ApiValidationError("Observaciones", "Las observaciones no pueden exceder 30 palabras."));
            }
        }

        return errors;
    }

    private static List<ApiValidationError> ValidateDeleteRequest(DeleteVacunoRequest request)
    {
        var errors = new List<ApiValidationError>();
        var reason = request.MotivoEliminacion?.Trim();

        if (string.IsNullOrWhiteSpace(reason))
        {
            errors.Add(new ApiValidationError(
                "MotivoEliminacion",
                "El motivo de eliminacion es obligatorio."));
        }
        else if (reason.Length > 250)
        {
            errors.Add(new ApiValidationError(
                "MotivoEliminacion",
                "El motivo de eliminacion no puede exceder 250 caracteres."));
        }

        return errors;
    }

    private static void Required(
        List<ApiValidationError> errors,
        string field,
        string? value,
        string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new ApiValidationError(field, message));
        }
    }

    private static void MaxLength(
        List<ApiValidationError> errors,
        string field,
        string? value,
        int maxLength,
        string message)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Trim().Length > maxLength)
        {
            errors.Add(new ApiValidationError(field, message));
        }
    }
}
