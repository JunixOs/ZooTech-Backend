using ZooTech.CodeGeneration.Generators;
using ZooTech.CodeGeneration.Models;

namespace ZooTech.CodeGeneration.UnitTests;

public sealed class SettingsGeneratorTests
{
    [Fact]
    public void Generate_EmptyCollection_ReturnsEmptySettingsClass()
    {
        var sut = new SettingsGenerator();
        var result = sut.Generate(Array.Empty<SettingMetadata>());

        Assert.Contains("public static class Settings", result);
        Assert.Contains("{", result);
        Assert.Contains("}", result);
    }

    [Fact]
    public void Generate_SingleGroupSingleSetting_ContainsSettingDefinition()
    {
        var settings = new List<SettingMetadata>
        {
            new() { GroupCode = "SECURITY", Code = "max_login_attempts", DataType = "INT" }
        };

        var sut = new SettingsGenerator();
        var result = sut.Generate(settings);

        Assert.Contains("public static class Security", result);
        Assert.Contains("SettingDefinition<int> MaxLoginAttempts", result);
        Assert.Contains("new(\"max_login_attempts\")", result);
    }

    [Fact]
    public void Generate_MultipleGroups_MixedTypes_CorrectOutput()
    {
        var settings = new List<SettingMetadata>
        {
            new() { GroupCode = "SECURITY", Code = "max_login_attempts", DataType = "INT" },
            new() { GroupCode = "SECURITY", Code = "enable_ssl", DataType = "BOOL" },
            new() { GroupCode = "EMAIL", Code = "smtp_host", DataType = "STRING" }
        };

        var sut = new SettingsGenerator();
        var result = sut.Generate(settings);

        Assert.Contains("public static class Security", result);
        Assert.Contains("SettingDefinition<int> MaxLoginAttempts", result);
        Assert.Contains("SettingDefinition<bool> EnableSsl", result);
        Assert.Contains("public static class Email", result);
        Assert.Contains("SettingDefinition<string> SmtpHost", result);
    }

    [Fact]
    public void MapType_Int_ReturnsInt()
    {
        var settings = new List<SettingMetadata>
        {
            new() { GroupCode = "T", Code = "test", DataType = "INT" }
        };

        var sut = new SettingsGenerator();
        var result = sut.Generate(settings);

        Assert.Contains("SettingDefinition<int>", result);
    }

    [Fact]
    public void MapType_Bool_ReturnsBool()
    {
        var settings = new List<SettingMetadata>
        {
            new() { GroupCode = "T", Code = "test", DataType = "BOOL" }
        };

        var sut = new SettingsGenerator();
        var result = sut.Generate(settings);

        Assert.Contains("SettingDefinition<bool>", result);
    }

    [Fact]
    public void MapType_String_ReturnsString()
    {
        var settings = new List<SettingMetadata>
        {
            new() { GroupCode = "T", Code = "test", DataType = "STRING" }
        };

        var sut = new SettingsGenerator();
        var result = sut.Generate(settings);

        Assert.Contains("SettingDefinition<string>", result);
    }

    [Fact]
    public void ToPascal_ConvertsSnakeCase()
    {
        var settings = new List<SettingMetadata>
        {
            new() { GroupCode = "MY_GROUP", Code = "my_setting_code", DataType = "STRING" }
        };

        var sut = new SettingsGenerator();
        var result = sut.Generate(settings);

        Assert.Contains("public static class MyGroup", result);
        Assert.Contains("MySettingCode", result);
    }

    [Fact]
    public void ToPascal_LeadingDigit_PrefixedWithUnderscore()
    {
        var settings = new List<SettingMetadata>
        {
            new() { GroupCode = "G", Code = "123_test", DataType = "STRING" }
        };

        var sut = new SettingsGenerator();
        var result = sut.Generate(settings);

        Assert.Contains("_123Test", result);
    }
}
