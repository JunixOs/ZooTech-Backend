using System;
using System.Linq;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.API.IntegrationTests.Seeders;

public static class BaseCatalogSeeder
{
    public const long GranjaId = 1;
    public const string RazaCode = "HOLSTEIN";
    public const string ColorCode = "BLANCO";
    public const string SexoHembraCode = "H";
    public const string SexoMachoCode = "M";
    public const string TipoAdquisicionCode = "NACIMIENTO";
    public const string TipoAdquisicionCompraCode = "COMPRA";
    public const string EstadoVacunoCode = "SANO";
    public const string EstadoVacunoEliminadoCode = "MUERTO";
    public const string UtilizacionCode = "PRODUCCION_LECHE";
    public const string VacunoModuleCode = "VACUNO";
    public const string PngExtensionCode = "PNG";

    public static void SeedBaseCatalogs(this GanaderiaDbContext ganaderiaDb)
    {
        if (!ganaderiaDb.granjas.Any(g => g.id == GranjaId))
        {
            var granja = new granja
            {
                id = GranjaId,
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

        if (!ganaderiaDb.cat_razas.Any(x => x.code == RazaCode))
            ganaderiaDb.cat_razas.Add(new cat_raza { code = RazaCode, nombre = "Holstein", activo = true });

        if (!ganaderiaDb.cat_colors.Any(x => x.code == ColorCode))
            ganaderiaDb.cat_colors.Add(new cat_color { code = ColorCode, nombre = "Blanco", activo = true });

        if (!ganaderiaDb.cat_sexos.Any(x => x.code == SexoHembraCode))
        {
            ganaderiaDb.cat_sexos.Add(new cat_sexo { code = SexoHembraCode, nombre = "Hembra" });
            ganaderiaDb.cat_sexos.Add(new cat_sexo { code = SexoMachoCode, nombre = "Macho" });
        }

        if (!ganaderiaDb.cat_tipo_adquisicions.Any(x => x.code == TipoAdquisicionCode))
            ganaderiaDb.cat_tipo_adquisicions.Add(new cat_tipo_adquisicion { code = TipoAdquisicionCode, nombre = "Nacimiento", activo = true });

        if (!ganaderiaDb.cat_tipo_adquisicions.Any(x => x.code == TipoAdquisicionCompraCode))
            ganaderiaDb.cat_tipo_adquisicions.Add(new cat_tipo_adquisicion { code = TipoAdquisicionCompraCode, nombre = "Compra", activo = true });

        if (!ganaderiaDb.cat_estado_vacunos.Any(x => x.code == EstadoVacunoCode))
            ganaderiaDb.cat_estado_vacunos.Add(new cat_estado_vacuno { code = EstadoVacunoCode, nombre = "Sano" });

        if (!ganaderiaDb.cat_estado_vacunos.Any(x => x.code == EstadoVacunoEliminadoCode))
            ganaderiaDb.cat_estado_vacunos.Add(new cat_estado_vacuno { code = EstadoVacunoEliminadoCode, nombre = "Muerto" });

        if (!ganaderiaDb.cat_tipo_utilizacions.Any(x => x.code == UtilizacionCode))
            ganaderiaDb.cat_tipo_utilizacions.Add(new cat_tipo_utilizacion { code = UtilizacionCode, nombre = "Produccion de leche", activo = true });

        if (!ganaderiaDb.cat_modulos.Any(x => x.code == VacunoModuleCode))
            ganaderiaDb.cat_modulos.Add(new cat_modulo { code = VacunoModuleCode, nombre = "Vacuno", activo = true });

        if (!ganaderiaDb.cat_tipo_archivos.Any(x => x.extension == PngExtensionCode))
            ganaderiaDb.cat_tipo_archivos.Add(new cat_tipo_archivo { extension = PngExtensionCode, mime_type = "image/png" });

        ganaderiaDb.SaveChanges();
    }
}
