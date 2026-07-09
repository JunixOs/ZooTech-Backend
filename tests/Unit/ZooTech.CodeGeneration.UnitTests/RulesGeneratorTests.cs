using ZooTech.CodeGeneration.Generators;
using ZooTech.CodeGeneration.Models;

namespace ZooTech.CodeGeneration.UnitTests;

public sealed class RulesGeneratorTests
{
    [Fact]
    public void Generate_EmptyCollection_ReturnsEmptyRulesClass()
    {
        var sut = new RulesGenerator();
        var result = sut.Generate(Array.Empty<RuleMetadata>());

        Assert.Contains("public static class Rules", result);
    }

    [Fact]
    public void Generate_SingleRule_ContainsRuleCode()
    {
        var rules = new List<RuleMetadata>
        {
            new() { Code = "AUTO_INVOICE" }
        };

        var sut = new RulesGenerator();
        var result = sut.Generate(rules);

        Assert.Contains("RuleCode AutoInvoice", result);
        Assert.Contains("new(\"AUTO_INVOICE\")", result);
    }

    [Fact]
    public void Generate_MultipleRules_AllPresent()
    {
        var rules = new List<RuleMetadata>
        {
            new() { Code = "AUTO_INVOICE" },
            new() { Code = "MANUAL_REVIEW" }
        };

        var sut = new RulesGenerator();
        var result = sut.Generate(rules);

        Assert.Contains("AutoInvoice", result);
        Assert.Contains("ManualReview", result);
    }
}
