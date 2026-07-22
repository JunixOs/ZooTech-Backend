using System;
using System.Linq;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.API.IntegrationTests.Seeders;

public static class BaseCatalogSeeder
{
    public static void SeedBaseCatalogs(this GanaderiaDbContext ganaderiaDb)
    {
        if (!ganaderiaDb.granjas.Any(g => g.id == 1))
        {
            var granja = new granja
            {
                id = 1,
                nombre = "Granja de Prueba",
                distrito_codigo = "010101",
                activo = true,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };
            
            var depto = new geo_departamento { codigo = "01", nombre = "Amazonas" };
            var prov = new geo_provincium { codigo = "0101", departamento_codigo = "01", nombre = "Chachapoyas", departamento_codigoNavigation = depto };
            var dist = new geo_distrito { codigo = "010101", provincia_codigo = "0101", nombre = "Chachapoyas", provincia_codigoNavigation = prov };
            
            granja.distrito_codigoNavigation = dist;
            ganaderiaDb.granjas.Add(granja);
        }
    }
}
