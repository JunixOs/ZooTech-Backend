using ZooTech.CodeGeneration.Generators;
using ZooTech.CodeGeneration.Models;

namespace ZooTech.CodeGeneration.UnitTests;

public sealed class FeaturesGeneratorTests
{
    [Fact]
    public void Generate_EmptyCollection_ReturnsEmptyFeaturesClass()
    {
        var sut = new FeaturesGenerator();
        var result = sut.Generate(Array.Empty<FeatureMetadata>());

        Assert.Contains("public static class Features", result);
    }

    [Fact]
    public void Generate_SingleFeature_ContainsFeatureCode()
    {
        var features = new List<FeatureMetadata>
        {
            new() { Code = "MODULE_INVENTORY" }
        };

        var sut = new FeaturesGenerator();
        var result = sut.Generate(features);

        Assert.Contains("FeatureCode ModuleInventory", result);
        Assert.Contains("new(\"MODULE_INVENTORY\")", result);
    }

    [Fact]
    public void Generate_MultipleFeatures_AllPresent()
    {
        var features = new List<FeatureMetadata>
        {
            new() { Code = "MODULE_INVENTORY" },
            new() { Code = "MODULE_BILLING" }
        };

        var sut = new FeaturesGenerator();
        var result = sut.Generate(features);

        Assert.Contains("ModuleInventory", result);
        Assert.Contains("ModuleBilling", result);
    }
}
