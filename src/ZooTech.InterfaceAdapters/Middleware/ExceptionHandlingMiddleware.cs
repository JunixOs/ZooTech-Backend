using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ZooTech.Application.Common.Exceptions;

namespace ZooTech.InterfaceAdapters.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(AppException ex)
            {
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = ex.StatusCode;

                var response = new ErrorResponse
                {
                    Error = new ErrorContent
                    {
                        Code = ex.Code,
                        Message = ex.Message,
                        Details = ex.Details
                    }
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response)
                );
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = 500;

                var response = new ErrorResponse
                {
                    Error = new ErrorContent
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "Ocurrio un error interno",
                        Details = []
                    }
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response)
                );
            }
        }
    }
}