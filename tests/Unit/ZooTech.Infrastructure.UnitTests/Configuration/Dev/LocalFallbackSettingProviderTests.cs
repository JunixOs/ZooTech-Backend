using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using ZooTech.Infrastructure.Configuration.Dev;

namespace ZooTech.Infrastructure.UnitTests.Configuration.Dev;

public class LocalFallbackSettingProviderTests
{
    [Fact]
    public async Task GetSettingAsync_CuandoExisteEnConfiguracionComoString_DebeRetornarValorDeConfiguracion()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?> {
            {"TenantSettings:REPORTS_DATE_FORMAT", "dd/MM/yyyy"}
        };
        
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var provider = new LocalFallbackSettingProvider(configuration);

        // Act
        var result = await provider.GetSettingAsync<string>("REPORTS_DATE_FORMAT", 1L);

        // Assert
        result.Should().Be("dd/MM/yyyy");
    }

    [Fact]
    public async Task GetSettingAsync_CuandoNoExisteEnConfiguracionYEsInt_DebeRetornarValorPorDefectoParseado()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var provider = new LocalFallbackSettingProvider(configuration);

        // Act
        var result = await provider.GetSettingAsync<int>("REPORTS_DEFAULT_DAYS", 1L);

        // Assert
        result.Should().Be(30); // Default is "30"
    }

    [Fact]
    public async Task GetSettingAsync_CuandoEsArrayDeStringsYNoExisteEnConfig_DebeRetornarArrayPorDefecto()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var provider = new LocalFallbackSettingProvider(configuration);

        // Act
        var result = await provider.GetSettingAsync<string[]>("REPORTS_ALLOWED_FORMATS", 1L);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo("json", "pdf", "excel");
    }

    [Fact]
    public async Task GetSettingAsync_CuandoNoExisteKeyNiDefault_DebeRetornarValorPorDefectoDelTipo()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder().Build();
        var provider = new LocalFallbackSettingProvider(configuration);

        // Act
        var resultString = await provider.GetSettingAsync<string>("MISSING_KEY", 1L);
        var resultInt = await provider.GetSettingAsync<int>("MISSING_KEY", 1L);

        // Assert
        resultString.Should().BeNull();
        resultInt.Should().Be(0);
    }
}
