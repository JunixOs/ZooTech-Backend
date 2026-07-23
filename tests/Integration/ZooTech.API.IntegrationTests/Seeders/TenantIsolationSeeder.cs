using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.API.IntegrationTests.Seeders;

public static class TenantIsolationSeeder
{
    public static string GetCode(string subdomain) => $"MT-{subdomain.ToUpperInvariant()}";

    public static long GetId(int tenantIndex) => 900_000L + tenantIndex;

    public static void SeedTenantMarker(
        this GanaderiaDbContext context,
        string subdomain,
        int tenantIndex)
    {
        var code = GetCode(subdomain);
        if (context.vacunos.Any(vacuno => vacuno.codigo == code))
        {
            return;
        }

        context.vacunos.Add(new vacuno
        {
            id = GetId(tenantIndex),
            codigo = code,
            nombre = $"Vacuno {subdomain}",
            fecha_nacimiento = new DateOnly(2022, 1, 1),
            tipo_adquisicion_code = "COMPRA",
            raza_code = "HOLSTEIN",
            color_code = "BLANCO",
            sexo_code = "H",
            granja_id = 1,
            fecha_registro = new DateOnly(2026, 1, 1),
            created_at = new DateTime(2026, 1, 1),
            updated_at = new DateTime(2026, 1, 1)
        });
    }
}
