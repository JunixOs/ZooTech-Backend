using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZooTech.Domain.Exceptions;

namespace ZooTech.InterfaceAdapters.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.BadRequest,
                "VALIDATION_ERROR",
                "Los datos enviados no son válidos.",
                ex.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    message = e.ErrorMessage
                }).ToArray()
            );
        }
        catch (VacunoYaExisteException ex)
        {
            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.Conflict,
                "VACUNO_ALREADY_EXISTS",
                ex.Message,
                Array.Empty<object>()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en la API.");

            var details = _environment.IsDevelopment()
                ? new object[]
                {
                    new
                    {
                        exception = ex.GetType().FullName,
                        message = ex.Message,
                        stackTrace = ex.ToString()
                    }
                }
                : Array.Empty<object>();

            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.InternalServerError,
                "INTERNAL_SERVER_ERROR",
                "Ocurrió un error interno. Por favor intente nuevamente.",
                details
            );
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string code,
        string message,
        object details)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = new
            {
                code,
                message,
                details
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}