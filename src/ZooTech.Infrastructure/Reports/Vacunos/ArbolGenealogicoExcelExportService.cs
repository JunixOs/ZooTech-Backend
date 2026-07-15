using ClosedXML.Excel;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities;
using ZooTech.Domain.Ganaderia.Module_Vacuno.Entities.GetArbolGenealogico;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class ArbolGenealogicoExcelExportService : IArbolGenealogicoExportService
{
    public Task<byte[]> GenerateExcelAsync(
        List<VacunoGenealogiaNode> arbolGenealogico, Vacuno raiz, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Árbol Genealógico");

        worksheet.Cell(1, 1).Value = "Nivel (Generación)";
        worksheet.Cell(1, 2).Value = "Parentesco";
        worksheet.Cell(1, 3).Value = "Código";
        worksheet.Cell(1, 4).Value = "Nombre";
        worksheet.Cell(1, 5).Value = "Raza";
        worksheet.Cell(1, 6).Value = "Sexo";
        worksheet.Cell(1, 7).Value = "Procedencia";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;

        var raizNodo = arbolGenealogico.FirstOrDefault(n => n.Vacuno.Id == raiz.Id);
        var queue = new Queue<(VacunoGenealogiaNode Node, List<string> Cadena)>();
        queue.Enqueue((raizNodo ?? new VacunoGenealogiaNode(raiz, 1, null), new List<string>()));
        var visited = new HashSet<long> { raiz.Id };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var v = current.Node.Vacuno;

            worksheet.Cell(row, 1).Value = current.Node.Nivel;
            worksheet.Cell(row, 2).Value = GetRelacion(current.Node.Nivel, current.Cadena);
            worksheet.Cell(row, 3).Value = v.Codigo;
            worksheet.Cell(row, 4).Value = v.Nombre;
            worksheet.Cell(row, 5).Value = v.RazaCode ?? "-";
            worksheet.Cell(row, 6).Value = v.SexoCode ?? "-";
            worksheet.Cell(row, 7).Value = current.Node.Procedencia ?? "-";
            row++;

            if (v.PadreId.HasValue && !visited.Contains(v.PadreId.Value))
            {
                var padre = arbolGenealogico.FirstOrDefault(x => x.Vacuno.Id == v.PadreId.Value);
                if (padre != null)
                {
                    visited.Add(padre.Vacuno.Id);
                    queue.Enqueue((padre, new List<string>(current.Cadena) { "Padre" }));
                }
            }

            if (v.MadreId.HasValue && !visited.Contains(v.MadreId.Value))
            {
                var madre = arbolGenealogico.FirstOrDefault(x => x.Vacuno.Id == v.MadreId.Value);
                if (madre != null)
                {
                    visited.Add(madre.Vacuno.Id);
                    queue.Enqueue((madre, new List<string>(current.Cadena) { "Madre" }));
                }
            }
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    private static string GetRelacion(int nivel, List<string> cadena)
    {
        if (nivel == 1) return "Raíz";
        if (nivel == 2) return cadena[^1]; 

        var esPaterno = cadena[0] == "Padre";
        var ladoTexto = esPaterno ? "paterno" : "materno";
        var ladoTextoF = esPaterno ? "paterna" : "materna";
        var esRamaFemenina = cadena[^1] == "Madre";

        return nivel switch
        {
            3 => esRamaFemenina ? $"Abuela {ladoTextoF}" : $"Abuelo {ladoTexto}",
            4 => esRamaFemenina ? $"Bisabuela {ladoTextoF}" : $"Bisabuelo {ladoTexto}",
            _ => $"Ancestro {ladoTexto} (nivel {nivel})"
        };
    }
}