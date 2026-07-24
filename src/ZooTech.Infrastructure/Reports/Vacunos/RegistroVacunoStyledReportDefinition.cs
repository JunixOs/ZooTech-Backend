using System.Globalization;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Infrastructure.Common.Export;

namespace ZooTech.Infrastructure.Reports.Vacunos;

internal static class RegistroVacunoStyledReportDefinition
{
    public static StyledDetailReport Create(
        RegistroVacunoDetalle vacuno,
        string extension,
        byte[]? imageContent = null)
        => new(
            new StyledReportMetadata(
                "Reporte de registro individual de vacuno",
                "Registro vacuno",
                $"registro_vacuno_{vacuno.Codigo}{extension}"),
            VacunoReportTheme.Styled,
            [
                new StyledDetailSection(
                    "Datos de identificación",
                    [
                        new("ID", vacuno.Id),
                        new("Código", vacuno.Codigo),
                        new("Nombre", vacuno.Nombre),
                        new("Fecha de nacimiento", vacuno.FechaNacimiento),
                        new("Sexo", vacuno.Sexo),
                        new("Raza", vacuno.Raza),
                        new("Color", vacuno.Color),
                        new("Estado", vacuno.Estado),
                        new("Fecha de registro", vacuno.FechaRegistro)
                    ]),
                new StyledDetailSection(
                    "Trazabilidad",
                    [
                        new("Código padre", vacuno.CodigoPadre),
                        new("Código madre", vacuno.CodigoMadre),
                        new("Código abuelo", vacuno.CodigoAbuelo),
                        new("Código abuela", vacuno.CodigoAbuela),
                        new("Granja", vacuno.Granja),
                        new("Distrito", vacuno.Distrito),
                        new("Provincia", vacuno.Provincia),
                        new("Departamento", vacuno.Departamento),
                        new("Procedencia", vacuno.Procedencia),
                        new("Adquisición por", vacuno.AdquisicionPor),
                        new(
                            "Precio de compra",
                            vacuno.PrecioCompra?.ToString("0.##", CultureInfo.InvariantCulture)),
                        new("Fecha de adquisición", vacuno.FechaAdquisicion)
                    ]),
                new StyledDetailSection(
                    "Especialización",
                    [
                        new("Apto para", vacuno.AptoPara),
                        new("Fecha de especificación", vacuno.FechaEspecificacion)
                    ]),
                new StyledDetailSection(
                    "Observaciones y auditoría",
                    [
                        new("Observaciones", vacuno.Observaciones),
                        new("Motivo del estado", vacuno.MotivoEstado),
                        new("Creado por", vacuno.CreadoPor),
                        new("Creado en", vacuno.CreadoEn),
                        new("Actualizado por", vacuno.ActualizadoPor),
                        new("Actualizado en", vacuno.ActualizadoEn)
                    ])
            ],
            imageContent);
}
