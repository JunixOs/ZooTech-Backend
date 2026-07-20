using System;
using System.Linq;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.API.IntegrationTests.Seeders;

public static class VacunosBasicSeeder
{
    public static void SeedVacunosBasic(this GanaderiaDbContext ganaderiaDb)
    {
        var hoyDateOnly = DateOnly.FromDateTime(DateTime.UtcNow);

        if (!ganaderiaDb.vacunos.Any(v => v.codigo == "VAC001"))
        {
            var hembra = new vacuno
            {
                codigo = "VAC001",
                nombre = "Vaca de Prueba",
                sexo_code = "H",
                fecha_nacimiento = new DateOnly(2020, 1, 1),
                tipo_adquisicion_code = "COMPRA",
                raza_code = "HOLSTEIN",
                color_code = "BLANCO",
                granja_id = 1,
                fecha_registro = hoyDateOnly,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };
            ganaderiaDb.vacunos.Add(hembra);
        }

        if (!ganaderiaDb.vacunos.Any(v => v.codigo == "VAC002"))
        {
            var macho = new vacuno
            {
                codigo = "VAC002",
                nombre = "Toro de Prueba",
                sexo_code = "M",
                fecha_nacimiento = new DateOnly(2020, 1, 1),
                tipo_adquisicion_code = "COMPRA",
                raza_code = "HOLSTEIN",
                color_code = "BLANCO",
                granja_id = 1,
                fecha_registro = hoyDateOnly,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };
            ganaderiaDb.vacunos.Add(macho);
        }
    }
}
