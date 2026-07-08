using ZooTech.CodeGeneration.Models;

namespace ZooTech.CodeGeneration.Generators;

public sealed class FeaturesGenerator
{
    public string Generate(IReadOnlyCollection<FeatureMetadata> features) =>
        GeneratorSupport.GenerateCodeDefinitions(
            features,
            wrapperClassName: "Features",
            targetTypeName: "FeatureCode",
            codeSelector: feature => feature.Code);
}
