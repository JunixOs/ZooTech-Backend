#pragma warning disable CS0618
using Moq;
using ZooTech.Application.Common.Gateway.Parametrization.Features;
using ZooTech.Application.Common.Gateway.Parametrization.Rules;
using ZooTech.Application.Common.Gateway.Parametrization.Settings;
using ZooTech.Domain.Configuration;

namespace ZooTech.Application.UnitTests;

public sealed class ProviderInterfaceTests
{
    [Fact]
    public async Task ISettingsProvider_GetAsync_WithSettingDefinition_InfersType()
    {
        var mock = new Mock<ISettingsProvider>();
        var setting = new SettingDefinition<int>("max_login_attempts");

        mock.Setup(p => p.GetAsync(1, setting))
            .ReturnsAsync(5);

        var result = await mock.Object.GetAsync(1, setting);

        Assert.IsType<int>(result);
        Assert.Equal(5, result);
    }

    [Fact]
    public async Task ISettingsProvider_GetAsync_StringSetting_ReturnsString()
    {
        var mock = new Mock<ISettingsProvider>();
        var setting = new SettingDefinition<string>("smtp_host");

        mock.Setup(p => p.GetAsync(1, setting))
            .ReturnsAsync("smtp.gmail.com");

        var result = await mock.Object.GetAsync(1, setting);

        Assert.IsType<string>(result);
        Assert.Equal("smtp.gmail.com", result);
    }

    [Fact]
    public async Task ISettingsProvider_RefreshAsync_Completes()
    {
        var mock = new Mock<ISettingsProvider>();
        mock.Setup(p => p.RefreshAsync(1)).Returns(Task.CompletedTask);

        await mock.Object.RefreshAsync(1);
    }

    [Fact]
    public async Task IFeatureProvider_IsEnabledAsync_ReturnsBool()
    {
        var mock = new Mock<IFeatureProvider>();
        var feature = new FeatureCode("MODULE_INVENTORY");

        mock.Setup(p => p.IsEnabledAsync(1, feature)).ReturnsAsync(true);
        mock.Setup(p => p.IsEnabledAsync(2, feature)).ReturnsAsync(false);

        Assert.True(await mock.Object.IsEnabledAsync(1, feature));
        Assert.False(await mock.Object.IsEnabledAsync(2, feature));
    }

    [Fact]
    public async Task IRuleProvider_IsEnabledAsync_ReturnsBool()
    {
        var mock = new Mock<IRuleProvider>();
        var rule = new RuleCode("AUTO_INVOICE");

        mock.Setup(p => p.IsEnabledAsync(1, rule)).ReturnsAsync(true);
        mock.Setup(p => p.IsEnabledAsync(2, rule)).ReturnsAsync(false);

        Assert.True(await mock.Object.IsEnabledAsync(1, rule));
        Assert.False(await mock.Object.IsEnabledAsync(2, rule));
    }
}
