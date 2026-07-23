using ZooTech.Application.Modules.Module_Vacuno.UseCases.ExportarArbolGenealogico;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

internal static class GenealogiaVacunoStyledReportDefinition
{
    public static StyledTabularReport<GenealogiaReportRow> Create(
        GenealogiaVacunoReportModel model,
        string extension)
        => new(
            new StyledReportMetadata(
                "Árbol genealógico",
                "Árbol genealógico",
                $"Genealogia_{model.Root.Id}{extension}",
                IsLandscape: true),
            VacunoReportTheme.Styled,
            GenealogiaReportRows.Build(model),
            [
                new("Nivel", row => row.Node.Nivel, 0.6f),
                new("Parentesco", row => row.Relationship, 1.5f),
                new("Código", row => row.Node.Vacuno.Codigo, 1.2f),
                new("Nombre", row => row.Node.Vacuno.Nombre, 1.8f),
                new("Raza", row => row.Node.Vacuno.RazaCode ?? "-", 1.2f),
                new("Sexo", row => row.Node.Vacuno.SexoCode ?? "-", 1),
                new("Procedencia", row => row.Node.Procedencia ?? "-", 1.8f)
            ],
            [
                new StyledReportSummary(
                    "Vacuno raíz",
                    $"{model.Root.Codigo} - {model.Root.Nombre}")
            ]);
}

internal sealed record GenealogiaReportRow(
    VacunoGenealogiaNode Node,
    string Relationship);

internal static class GenealogiaReportRows
{
    public static IReadOnlyCollection<GenealogiaReportRow> Build(
        GenealogiaVacunoReportModel model)
    {
        var nodesById = model.Nodes.ToDictionary(node => node.Vacuno.Id);
        var rootNode = nodesById.GetValueOrDefault(model.Root.Id)
            ?? new VacunoGenealogiaNode(model.Root, 1, null);
        var rows = new List<GenealogiaReportRow>();
        var queue = new Queue<(VacunoGenealogiaNode Node, IReadOnlyList<string> Path)>();
        var visited = new HashSet<long> { model.Root.Id };
        queue.Enqueue((rootNode, Array.Empty<string>()));

        while (queue.Count > 0)
        {
            var (node, path) = queue.Dequeue();
            rows.Add(new GenealogiaReportRow(node, GetRelationship(node.Nivel, path)));

            EnqueueParent(node.Vacuno.PadreId, "Padre", path, nodesById, visited, queue);
            EnqueueParent(node.Vacuno.MadreId, "Madre", path, nodesById, visited, queue);
        }

        return rows;
    }

    private static void EnqueueParent(
        long? parentId,
        string relationship,
        IReadOnlyList<string> path,
        IReadOnlyDictionary<long, VacunoGenealogiaNode> nodesById,
        ISet<long> visited,
        Queue<(VacunoGenealogiaNode Node, IReadOnlyList<string> Path)> queue)
    {
        if (!parentId.HasValue ||
            !visited.Add(parentId.Value) ||
            !nodesById.TryGetValue(parentId.Value, out var parent))
        {
            return;
        }

        var currentPath = new List<string>(path) { relationship };
        queue.Enqueue((parent, currentPath));
    }

    private static string GetRelationship(int level, IReadOnlyList<string> path)
    {
        if (level == 1 || path.Count == 0)
        {
            return "Raíz";
        }

        if (level == 2)
        {
            return path[^1];
        }

        var paternal = path[0] == "Padre";
        var female = path[^1] == "Madre";
        var side = paternal ? "paterno" : "materno";
        var femaleSide = paternal ? "paterna" : "materna";

        return level switch
        {
            3 => female ? $"Abuela {femaleSide}" : $"Abuelo {side}",
            4 => female ? $"Bisabuela {femaleSide}" : $"Bisabuelo {side}",
            _ => $"Ancestro {side} (nivel {level})"
        };
    }
}
