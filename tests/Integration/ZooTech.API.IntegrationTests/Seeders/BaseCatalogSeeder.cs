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

        if (!ganaderiaDb.cat_tipo_fecundacions.Any(t => t.code == "MN"))
            ganaderiaDb.cat_tipo_fecundacions.Add(new cat_tipo_fecundacion { nombre = "Monta Natural", code = "MN" });

        if (!ganaderiaDb.cat_resultado_fecundacions.Any(r => r.code == "POSITIVO"))
            ganaderiaDb.cat_resultado_fecundacions.Add(new cat_resultado_fecundacion { nombre = "Positivo", code = "POSITIVO" });

        if (!ganaderiaDb.cat_razas.Any(r => r.code == "HOLSTEIN"))
            ganaderiaDb.cat_razas.Add(new cat_raza { code = "HOLSTEIN", nombre = "Holstein", activo = true });

        if (!ganaderiaDb.cat_sexos.Any(s => s.code == "H"))
        {
            ganaderiaDb.cat_sexos.Add(new cat_sexo { code = "H", nombre = "Hembra" });
            ganaderiaDb.cat_sexos.Add(new cat_sexo { code = "M", nombre = "Macho" });
        }

        if (!ganaderiaDb.cat_colors.Any(c => c.code == "BLANCO"))
            ganaderiaDb.cat_colors.Add(new cat_color { code = "BLANCO", nombre = "Blanco", activo = true });

        if (!ganaderiaDb.cat_tipo_adquisicions.Any(ta => ta.code == "COMPRA"))
            ganaderiaDb.cat_tipo_adquisicions.Add(new cat_tipo_adquisicion { code = "COMPRA", nombre = "Compra", activo = true });

        if (!ganaderiaDb.cat_tipo_utilizacions.Any(tu => tu.code == "LECHE"))
            ganaderiaDb.cat_tipo_utilizacions.Add(new cat_tipo_utilizacion { code = "LECHE", nombre = "Leche", activo = true });

        ganaderiaDb.SaveChanges();
    }
}
