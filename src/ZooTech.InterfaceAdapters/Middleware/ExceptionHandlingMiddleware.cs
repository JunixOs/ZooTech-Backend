using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Celo.UseCases.FecundacionEstado.Common;
using ZooTech.Application.Modules.Module_Vacuno.Exceptions;
using ZooTech.Application.Modules.Module_Fecundacion.Exceptions;
using ZooTech.InterfaceAdapters.DTOs.Responses;


namespace ZooTech.InterfaceAdapters.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var details = ex.Errors.Select(error => new ErrorDetail
            {
                Field = error.PropertyName,
                Message = error.ErrorMessage
            });

            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Los datos enviados no son validos.",
                details);
        }
        catch (FecundacionEstadoValidationException ex)
        {
            var details = ex.Errors.Select(error => new ErrorDetail
            {
                Field = error.Key,
                Message = error.Value
            });

            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "Los datos enviados no son validos.",
                details);
        }
        catch (VacunoException ex)
        {
            await WriteErrorAsync(
                context,
                ex.StatusCode,
                ex.ErrorCode,
                ex.Message);
        }
        catch (FecundacionException ex)
        {
            await WriteErrorAsync(
                context,
                ex.StatusCode,
                ex.ErrorCode,
                ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status404NotFound,
                "NOT_FOUND",
                ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status409Conflict,
                "CONFLICT",
                ex.Message);
        }
        catch (BusinessException ex)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                ex.Code,
                ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                "BAD_REQUEST",
                ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status401Unauthorized,
                "UNAUTHORIZED",
                ex.Message);
        }
        catch (OperationCanceledException)
        {
            await WriteErrorAsync(
                context,
                499,
                "REQUEST_CANCELED",
                "La solicitud fue cancelada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado procesando la solicitud.");

            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "INTERNAL_SERVER_ERROR",
                "Ocurrio un error inesperado.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message,
        IEnumerable<ErrorDetail>? details = null)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(
            ErrorResponse.Create(code, message, details),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}
