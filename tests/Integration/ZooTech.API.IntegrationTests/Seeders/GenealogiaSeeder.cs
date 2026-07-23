using System;
using System.Linq;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.API.IntegrationTests.Seeders;

public static class GenealogiaSeeder
{
    public const string TargetCode = "V001";

    public static void SeedGenealogia(this GanaderiaDbContext ganaderiaDb)
    {
        var hoyDateOnly = DateOnly.FromDateTime(DateTime.UtcNow);
        
        if (!ganaderiaDb.vacunos.Any(v => v.codigo == TargetCode))
        {
            // Abuelos
            ganaderiaDb.vacunos.Add(new vacuno { id = 201, codigo = "M001", nombre = "Abuelo", sexo_code = "M", fecha_nacimiento = new DateOnly(2010, 1, 1), tipo_adquisicion_code = "COMPRA", raza_code = "HOLSTEIN", color_code = "BLANCO", granja_id = 1, fecha_registro = hoyDateOnly, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow });
            ganaderiaDb.vacunos.Add(new vacuno { id = 202, codigo = "H001", nombre = "Abuela", sexo_code = "H", fecha_nacimiento = new DateOnly(2010, 1, 1), tipo_adquisicion_code = "COMPRA", raza_code = "HOLSTEIN", color_code = "BLANCO", granja_id = 1, fecha_registro = hoyDateOnly, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow });
            
            // Padres
            ganaderiaDb.vacunos.Add(new vacuno { id = 203, codigo = "M002", nombre = "Padre", padre_id = 201, madre_id = 202, sexo_code = "M", fecha_nacimiento = new DateOnly(2015, 1, 1), tipo_adquisicion_code = "COMPRA", raza_code = "HOLSTEIN", color_code = "BLANCO", granja_id = 1, fecha_registro = hoyDateOnly, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow });
            ganaderiaDb.vacunos.Add(new vacuno { id = 204, codigo = "H002", nombre = "Madre", sexo_code = "H", fecha_nacimiento = new DateOnly(2015, 1, 1), tipo_adquisicion_code = "COMPRA", raza_code = "HOLSTEIN", color_code = "BLANCO", granja_id = 1, fecha_registro = hoyDateOnly, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow });
            
            // Hijo Principal (Objetivo de la prueba)
            ganaderiaDb.vacunos.Add(new vacuno { id = 205, codigo = TargetCode, nombre = "Hijo Principal", padre_id = 203, madre_id = 204, sexo_code = "H", fecha_nacimiento = new DateOnly(2020, 1, 1), tipo_adquisicion_code = "COMPRA", raza_code = "HOLSTEIN", color_code = "BLANCO", granja_id = 1, fecha_registro = hoyDateOnly, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow });
            
            // Hijo Extra (Para probar que la eliminación no afecte el árbol si no es padre)
            ganaderiaDb.vacunos.Add(new vacuno { id = 206, codigo = "DEL1", nombre = "Hijo Eliminado", padre_id = 203, madre_id = 204, sexo_code = "H", fecha_nacimiento = new DateOnly(2021, 1, 1), tipo_adquisicion_code = "COMPRA", raza_code = "HOLSTEIN", color_code = "BLANCO", granja_id = 1, fecha_registro = hoyDateOnly, deleted_at = DateTime.UtcNow, created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow });
        }
    }
}
