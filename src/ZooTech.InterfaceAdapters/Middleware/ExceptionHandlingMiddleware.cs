using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Domain.Shared.Exceptions;
using ZooTech.InterfaceAdapters.Models;
using ZooTech.InterfaceAdapters.Utils;

namespace ZooTech.InterfaceAdapters.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next, 
            ILogger<ExceptionHandlingMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IAppAuditService appAuditService
        )
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogDebug(
                    "Request canceled by the client: {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);
            }
            catch (AppDomainException ex)
            {
                _logger.LogWarning(ex, "ZooTechException: {Code} - {Type} - {Message} - {Scope}", ex.ErrorCode.ToString(), ex.ErrorType.ToString(), ex.Message.ToString(), ex.ScopeName.ToString());

                var rootException = ex.GetBaseException();
                
                await appAuditService.AuditErrorAsync(
                    new AuditErrorInfo
                    {
                        EventType = AuditEventType.ZooTechException,
                        CustomMessage =  $"ZooTechException: {ex.ErrorCode} - {ex.ScopeName} - {ex.ErrorType} - {ex.Message}",

                        ExceptionType = ex.GetType().FullName,
                        RootExceptionType = rootException.GetType().Name,
                        
                        Message = ex.Message,
                        RootMessage = ExceptionExtensions.ToAuditMessage(rootException),
                        
                        Source = rootException.Source,

                        DeclaringType = rootException.TargetSite?.DeclaringType?.FullName,
                        Method = rootException.TargetSite?.Name,

                        AppInformation = new ErrorAppInformation
                        {
                            ErrorCode = ex.ErrorCode,
                            ScopeName = ex.ScopeName,
                            ModuleName = ex.ModuleName,
                            Details = ex.Details,
                            CompleteErrorCode = ex.CompleteErrorCode
                        },

                        StackTrace = rootException.StackTrace
                    }
                );
                
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = (int)ToHttpStatusCode.Convert(ex.ErrorType);

                var response = new ErrorResponseModel
                {
                    Error = new ErrorContent
                    {
                        ErrorCode = ex.CompleteErrorCode,
                        Message = ex.Message,
                        Details = ex.Details,
                        FieldErrors = ex is IFieldValidationException validationException
                            ? validationException.FieldErrors
                                .Select(error => new FieldErrorContent(error.Field, error.Code, error.Message))
                                .ToList()
                            : []
                    }
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                var rootException = ex.GetBaseException();

                _logger.LogError(rootException, "Unhandled exception");

                await appAuditService.AuditErrorAsync(
                    new AuditErrorInfo
                    {
                        EventType = AuditEventType.UnhandledException,
                        CustomMessage =  "Unhandled exception",

                        ExceptionType = ex.GetType().FullName,
                        RootExceptionType = rootException.GetType().Name,
                        
                        Message = ExceptionExtensions.ToAuditMessage(rootException),
                        RootMessage = ExceptionExtensions.ToAuditMessage(rootException),

                        Source = rootException.Source,
                        
                        DeclaringType = rootException.TargetSite?.DeclaringType?.FullName,
                        Method = rootException.TargetSite?.Name,

                        StackTrace = rootException.StackTrace
                    }
                );

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

                await context.Response.WriteAsJsonAsync(response);
            }
        }


    }
}
