using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.API.IntegrationTests.Seeders;

public static class VacunosReportesSeeder
{
    public static void SeedVacunosReportes(this GanaderiaDbContext ganaderiaDb)
    {
        if (ganaderiaDb.vacunos.Any(vacuno => vacuno.codigo.StartsWith("RPT-")))
        {
            return;
        }

        var now = DateTime.UtcNow;
        var reportVacunos = Enumerable.Range(1, 205)
            .Select(index => new vacuno
            {
                id = 1000 + index,
                codigo = $"RPT-{index:000}",
                nombre = $"Vacuno Reporte {index:000}",
                sexo_code = index % 2 == 0 ? "H" : "M",
                fecha_nacimiento = new DateOnly(2020, 1, 1),
                tipo_adquisicion_code = "COMPRA",
                raza_code = "HOLSTEIN",
                color_code = "BLANCO",
                granja_id = 1,
                fecha_registro = DateOnly.FromDateTime(now),
                created_at = now,
                updated_at = now
            });

        ganaderiaDb.vacunos.AddRange(reportVacunos);
    }
}
