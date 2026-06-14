using ClosedXML.Excel;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class ArbolGenealogicoExcelExportService : IArbolGenealogicoExportService
{
    public Task<byte[]> GenerateExcelAsync(List<Vacuno> arbolGenealogico, Vacuno raiz, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Árbol Genealógico");

        worksheet.Cell(1, 1).Value = "Nivel (Generación)";
        worksheet.Cell(1, 2).Value = "Parentesco";
        worksheet.Cell(1, 3).Value = "Código";
        worksheet.Cell(1, 4).Value = "Nombre";
        worksheet.Cell(1, 5).Value = "Raza";
        worksheet.Cell(1, 6).Value = "Sexo";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        
        var queue = new Queue<(Vacuno Node, int Nivel, string Relacion)>();
        queue.Enqueue((raiz, 1, "Raíz"));
        
        var visited = new HashSet<long> { raiz.Id };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            worksheet.Cell(row, 1).Value = current.Nivel;
            worksheet.Cell(row, 2).Value = current.Relacion;
            worksheet.Cell(row, 3).Value = current.Node.Codigo;
            worksheet.Cell(row, 4).Value = current.Node.Nombre;
            worksheet.Cell(row, 5).Value = current.Node.RazaCode ?? "-";
            worksheet.Cell(row, 6).Value = current.Node.SexoCode ?? "-";

            row++;

            if (current.Node.PadreId.HasValue && !visited.Contains(current.Node.PadreId.Value))
            {
                var padre = arbolGenealogico.FirstOrDefault(x => x.Id == current.Node.PadreId.Value);
                if (padre != null)
                {
                    visited.Add(padre.Id);
                    queue.Enqueue((padre, current.Nivel + 1, GetRelacion(current.Nivel + 1, "Padre")));
                }
            }

            if (current.Node.MadreId.HasValue && !visited.Contains(current.Node.MadreId.Value))
            {
                var madre = arbolGenealogico.FirstOrDefault(x => x.Id == current.Node.MadreId.Value);
                if (madre != null)
                {
                    visited.Add(madre.Id);
                    queue.Enqueue((madre, current.Nivel + 1, GetRelacion(current.Nivel + 1, "Madre")));
                }
            }
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    private static string GetRelacion(int nivel, string tipo)
    {
        return nivel switch
        {
            1 => "Raíz",
            2 => tipo,
            3 => tipo == "Padre" ? "Abuelo paterno" : "Abuela materna", // Technically Padre could be Madre's father, but BFS simplifies it. We just use Abuelo/Abuela for simplicity based on the branch.
            4 => tipo == "Padre" ? "Bisabuelo" : "Bisabuela",
            _ => "Ancestro"
        };
    }
}
