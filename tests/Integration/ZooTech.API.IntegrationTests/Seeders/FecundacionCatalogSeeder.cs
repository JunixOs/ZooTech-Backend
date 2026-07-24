using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.API.IntegrationTests.Seeders;

public static class FecundacionCatalogSeeder
{
    public const string TipoCode = "MONTA_NATURAL";
    public const string TipoApiCode = "monta_natural";
    public const string ResultadoCode = "PENDIENTE";
    public const string ResultadoApiCode = "pendiente_confirmacion";
    public const string EstadoCode = "EN_ESPERA";

    public static void SeedFecundacionCatalogs(this GanaderiaDbContext context)
    {
        if (!context.cat_tipo_fecundacions.Any(x => x.code == TipoCode))
        {
            context.cat_tipo_fecundacions.Add(new cat_tipo_fecundacion
            {
                code = TipoCode,
                nombre = "Monta Natural"
            });
        }

        if (!context.cat_resultado_fecundacions.Any(x => x.code == ResultadoCode))
        {
            context.cat_resultado_fecundacions.Add(new cat_resultado_fecundacion
            {
                code = ResultadoCode,
                nombre = "Pendiente"
            });
        }

        if (!context.cat_estado_fecundacion_vacunos.Any(x => x.code == EstadoCode))
        {
            context.cat_estado_fecundacion_vacunos.Add(new cat_estado_fecundacion_vacuno
            {
                code = EstadoCode,
                nombre = "En espera"
            });
        }
    }
}
