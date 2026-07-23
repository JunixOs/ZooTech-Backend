using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ZooTech.Infrastructure.Persistence.Context;

#nullable disable

namespace ZooTech.Infrastructure.Persistence.Migrations.Ganaderia;

[DbContext(typeof(GanaderiaDbContext))]
[Migration("20260723215000_SeedVacunoPhotoCatalogs")]
public sealed class SeedVacunoPhotoCatalogs : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF NOT EXISTS (
                SELECT 1
                FROM cat_modulo
                WHERE activo = 1
                  AND (
                      UPPER(code) IN ('VACUNO', 'VACUNOS')
                      OR UPPER(nombre) LIKE '%VACUNO%'
                  )
            )
            BEGIN
                INSERT INTO cat_modulo (code, nombre, descripcion, activo)
                VALUES ('VACUNO', 'Vacuno', 'Archivos del modulo Vacuno', 1);
            END;

            IF NOT EXISTS (
                SELECT 1
                FROM cat_tipo_archivo
                WHERE LOWER(mime_type) = 'image/png'
                   OR LOWER(extension) IN ('.png', 'png')
            )
            BEGIN
                INSERT INTO cat_tipo_archivo (extension, mime_type)
                VALUES ('.png', 'image/png');
            END;

            IF NOT EXISTS (
                SELECT 1
                FROM cat_tipo_archivo
                WHERE LOWER(mime_type) IN ('image/jpeg', 'image/jpg')
                   OR LOWER(extension) IN ('.jpg', 'jpg', '.jpeg', 'jpeg')
            )
            BEGIN
                INSERT INTO cat_tipo_archivo (extension, mime_type)
                VALUES ('.jpg', 'image/jpeg');
            END;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DELETE FROM cat_tipo_archivo
            WHERE extension IN ('.png', '.jpg')
              AND NOT EXISTS (
                  SELECT 1
                  FROM archivo
                  WHERE archivo.extension = cat_tipo_archivo.extension
              );

            DELETE FROM cat_modulo
            WHERE code = 'VACUNO'
              AND nombre = 'Vacuno'
              AND NOT EXISTS (
                  SELECT 1
                  FROM archivo
                  WHERE archivo.modulo_code = cat_modulo.code
              );
            """);
    }
}
