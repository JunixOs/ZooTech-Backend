using ZooTech.Domain.Configuration;

namespace ZooTech.Domain.UnitTests.Configuration;

public sealed class ConfigurationModelsTests
{
    [Fact]
    public void SettingDefinition_StoresCode()
    {
        var sut = new SettingDefinition<int>("test_code");
        Assert.Equal("test_code", sut.Code);
    }

    [Fact]
    public void SettingDefinition_DifferentTypeParams_AreDifferentTypes()
    {
        var intDef = new SettingDefinition<int>("x");
        var stringDef = new SettingDefinition<string>("x");

        Assert.Equal(typeof(SettingDefinition<int>), intDef.GetType());
        Assert.Equal(typeof(SettingDefinition<string>), stringDef.GetType());
    }

    [Fact]
    public void FeatureCode_StoresValue()
    {
        var sut = new FeatureCode("MODULE_TEST");
        Assert.Equal("MODULE_TEST", sut.Value);
    }

    [Fact]
    public void FeatureCode_EqualityByValue()
    {
        var a = new FeatureCode("X");
        var b = new FeatureCode("X");
        var c = new FeatureCode("Y");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }

    [Fact]
    public void RuleCode_StoresValue()
    {
        var sut = new RuleCode("RULE_TEST");
        Assert.Equal("RULE_TEST", sut.Value);
    }

    [Fact]
    public void RuleCode_EqualityByValue()
    {
        var a = new RuleCode("X");
        var b = new RuleCode("X");
        var c = new RuleCode("Y");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }

    [Fact]
    public void Settings_Vacunos_DefaultFilterDays_IsSettingDefinitionOfInt()
    {
        var sut = Settings.Vacunos.VacunosDefaultFilterDays;
        Assert.IsType<SettingDefinition<int>>(sut);
        Assert.Equal("VACUNOS_DEFAULT_FILTER_DAYS", sut.Code);
    }

    [Fact]
    public void Features_ModuleVacunos_IsFeatureCode()
    {
        var sut = Features.ModuleVacunos;
        Assert.Equal("MODULE_VACUNOS", sut.Value);
    }

    [Fact]
    public void Rules_VacunosEliminacionCondicionada_IsRuleCode()
    {
        var sut = Rules.VacunosEliminacionCondicionada;
        Assert.Equal("VACUNOS_ELIMINACION_CONDICIONADA", sut.Value);
    }
}
