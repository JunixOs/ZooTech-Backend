using Microsoft.Extensions.Configuration;
using ZooTech.CodeGeneration.Generators;
using ZooTech.CodeGeneration.Readers;
using ZooTech.CodeGeneration.Writers;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

var connectionString = configuration.GetConnectionString("ControlPlane")
    ?? throw new InvalidOperationException("Connection string 'ControlPlane' not found.");

// Resolve paths relative to the CodeGeneration project directory (deterministic regardless of CWD)
// AppContext.BaseDirectory = .../ZooTech.CodeGeneration/bin/Debug/net10.0/
// 3 levels up = .../ZooTech.CodeGeneration/
var projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
// 2 levels up from projectDir = solution root
var domainConfigDir = Path.GetFullPath(Path.Combine(projectDir, "..", "..", "src", "ZooTech.Domain", "Configuration"));

var reader = new MetadataReader(connectionString);

IReadOnlyCollection<ZooTech.CodeGeneration.Models.SettingMetadata> settings;
IReadOnlyCollection<ZooTech.CodeGeneration.Models.FeatureMetadata> features;
IReadOnlyCollection<ZooTech.CodeGeneration.Models.RuleMetadata> rules;

try
{
    settings = await reader.GetSettingsAsync();
    features = await reader.GetFeaturesAsync();
    rules = await reader.GetRulesAsync();

    Console.WriteLine($"  Settings found: {settings.Count}");
    Console.WriteLine($"  Features found: {features.Count}");
    Console.WriteLine($"  Rules found: {rules.Count}");
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Could not read metadata from database ({ex.Message}). Generating empty files.");
    settings = Array.Empty<ZooTech.CodeGeneration.Models.SettingMetadata>();
    features = Array.Empty<ZooTech.CodeGeneration.Models.FeatureMetadata>();
    rules = Array.Empty<ZooTech.CodeGeneration.Models.RuleMetadata>();
}

var writer = new FileWriter();

writer.Write(
    Path.Combine(domainConfigDir, "Settings.g.cs"),
    new SettingsGenerator().Generate(settings));

writer.Write(
    Path.Combine(domainConfigDir, "Features.g.cs"),
    new FeaturesGenerator().Generate(features));

writer.Write(
    Path.Combine(domainConfigDir, "Rules.g.cs"),
    new RulesGenerator().Generate(rules));

Console.WriteLine($"Code generation completed. Output: {domainConfigDir}");
