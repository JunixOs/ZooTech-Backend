using ZooTech.CodeGeneration.Models;

namespace ZooTech.CodeGeneration.Generators;

public sealed class RulesGenerator
{
    public string Generate(IReadOnlyCollection<RuleMetadata> rules) =>
        GeneratorSupport.GenerateCodeDefinitions(
            rules,
            wrapperClassName: "Rules",
            targetTypeName: "RuleCode",
            codeSelector: rule => rule.Code);
}
