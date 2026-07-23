using System;
using System.Linq;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.API.IntegrationTests.Seeders;

public static class VacunosBasicSeeder
{
    public const string HembraCode = "VAC001";
    public const string MachoCode = "VAC002";

    public static void SeedVacunosBasic(this GanaderiaDbContext ganaderiaDb)
    {
        var hoyDateOnly = DateOnly.FromDateTime(DateTime.UtcNow);

        if (!ganaderiaDb.vacunos.Any(v => v.codigo == HembraCode))
        {
            var hembra = new vacuno
            {
                codigo = HembraCode,
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

        if (!ganaderiaDb.vacunos.Any(v => v.codigo == MachoCode))
        {
            var macho = new vacuno
            {
                codigo = MachoCode,
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
