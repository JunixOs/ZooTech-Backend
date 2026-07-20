using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Auditing;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Middleware;

namespace ZooTech.InterfaceAdapters.UnitTests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private static ExceptionHandlingMiddleware CreateMiddleware(RequestDelegate next)
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        return new ExceptionHandlingMiddleware(next, loggerMock.Object);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }

    [Fact]
    public async Task Should_Return_Json_For_AppException()
    {
        // Arrange
        var middleware = CreateMiddleware(_ => throw new ValidationException(
            new List<string> { "ERROR" },
            ScopeName.Application,
            ModuleName.Tenancing,
            [new FieldValidationError("codigo", "ERROR", "Campo invalido.")]));
        var context = CreateHttpContext();

        var auditServiceMock = new Mock<IAppAuditService>();

        // Act
        await middleware.InvokeAsync(context , auditServiceMock.Object);

        // Assert
        context.Response.StatusCode.Should().Be(400);
        context.Response.ContentType.Should().StartWith("application/json");
        var body = await ReadResponseBody(context);
        body.Should().Contain("VALIDATION_ERROR");
        body.Should().Contain("fieldErrors");
        body.Should().Contain("codigo");
    }

    [Fact]
    public async Task Should_Return_500_For_Generic_Exception()
    {
        // Arrange
        var middleware = CreateMiddleware(_ => throw new Exception("Boom"));
        var context = CreateHttpContext();

        var auditServiceMock = new Mock<IAppAuditService>();

        // Act
        await middleware.InvokeAsync(context , auditServiceMock.Object);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        var body = await ReadResponseBody(context);
        body.Should().Contain("INTERNAL_SERVER_ERROR");
    }
}
