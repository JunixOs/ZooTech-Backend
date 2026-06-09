using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using ZooTech.Application.Common.Exceptions;
using ZooTech.InterfaceAdapters.Middleware;

namespace ZooTech.InterfaceAdapters.UnitTests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private class TestAppException : AppException
    {
        public TestAppException(string code, string message) : base(code, message, ["campo_invalido"]) { }
        public override int StatusCode => 400;
    }

    [Fact]
    public async Task InvokeAsync_CuandoArrojaAppException_DebeRetornarJsonYStatusCodeCorrecto()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var expectedException = new TestAppException("BAD_REQUEST", "Error de validacion");

        var middleware = new ExceptionHandlingMiddleware((innerHttpContext) =>
        {
            throw expectedException;
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseText = await reader.ReadToEndAsync();
        
        var parsedResponse = JsonSerializer.Deserialize<JsonElement>(responseText);
        
        parsedResponse.GetProperty("Error").GetProperty("Code").GetString().Should().Be("BAD_REQUEST");
        parsedResponse.GetProperty("Error").GetProperty("Message").GetString().Should().Be("Error de validacion");
        parsedResponse.GetProperty("Error").GetProperty("Details").GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task InvokeAsync_CuandoArrojaExcepcionNoControlada_DebeRetornar500InternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware((innerHttpContext) =>
        {
            throw new InvalidOperationException("Error critico del sistema.");
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseText = await reader.ReadToEndAsync();
        
        var parsedResponse = JsonSerializer.Deserialize<JsonElement>(responseText);
        
        parsedResponse.GetProperty("Error").GetProperty("Code").GetString().Should().Be("INTERNAL_SERVER_ERROR");
        parsedResponse.GetProperty("Error").GetProperty("Message").GetString().Should().Be("Ocurrio un error interno");
        parsedResponse.GetProperty("Error").GetProperty("Details").GetArrayLength().Should().Be(0);
    }
}
