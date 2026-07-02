using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZooTech.Domain.Shared.Exceptions;
using ZooTech.InterfaceAdapters.Models;
using ZooTech.InterfaceAdapters.Utils;

namespace ZooTech.InterfaceAdapters.Middleware
{
    public class ExceptionHandlingMiddleware
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
            catch (AppDomainException ex)
            {
                _logger.LogWarning(ex, "ApplicationException: {Code} - {Message}", ex.ErrorType, ex.Message);

                context.Response.ContentType = "application/json";

                context.Response.StatusCode = (int)ToHttpStatusCode.Convert(ex.ErrorType);

                var response = new ErrorResponseModel
                {
                    Error = new ErrorContent
                    {
                        ErrorCode = ex.ErrorCode,
                        Message = ex.Message,
                        Details = ex.Details
                    }
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var response = new ErrorResponseModel
                {
                    Error = new ErrorContent
                    {
                        ErrorCode = "INTERNAL_SERVER_ERROR",
                        Message = "Ocurrio un error interno",
                        Details = []
                    }
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
