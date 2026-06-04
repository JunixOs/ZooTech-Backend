using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Exceptions;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ApplicationRuleException ex)
        {
            await WriteErrorAsync(context, ex.StatusCode, ex.Code, ex.Message, ex.Details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await WriteErrorAsync(
                context,
                (int)HttpStatusCode.InternalServerError,
                "INTERNAL_SERVER_ERROR",
                "Error interno del servidor.",
                []);
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message,
        IReadOnlyCollection<ApplicationErrorDetail> details)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponseDto(
            new ErrorBodyDto(
                code,
                message,
                details.Select(detail => new ErrorDetailDto(detail.Field, detail.Message)).ToList()));

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await context.Response.WriteAsync(json);
    }
}
