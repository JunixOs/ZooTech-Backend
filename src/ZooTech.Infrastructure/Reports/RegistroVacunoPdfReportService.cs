using System.Globalization;
using System.Net;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Infrastructure.Storage;

namespace ZooTech.Infrastructure.Reports;

public sealed class RegistroVacunoPdfReportService : IRegistroVacunoPdfReportService
{
    private readonly ReportStorageOptions _storageOptions;

    public RegistroVacunoPdfReportService(IOptions<ReportStorageOptions> storageOptions)
    {
        _storageOptions = storageOptions.Value;
    }

    public Task<RegistroVacunoPdfReportResult> GenerateAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default)
    {
        var culture = CultureInfo.InvariantCulture;
        var namePattern = "reporte_{0}_{1}.pdf";
        var codigo = SanitizeFileNamePart(vacuno.Codigo);
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd", culture);
        var fileName = string.Format(culture, namePattern, codigo, fecha);

        var outputDirectory = Path.Combine(
            AppContext.BaseDirectory,
            _storageOptions.ReportesBasePath,
            _storageOptions.ReportesVacunosPath);

        Directory.CreateDirectory(outputDirectory);
        var filePath = Path.Combine(outputDirectory, fileName);

        var document = CreateDocument(vacuno, culture);
        document.GeneratePdf(filePath);

        var downloadUrl = $"{_storageOptions.ReportesUrlBase}{Uri.EscapeDataString(fileName)}";
        return Task.FromResult(new RegistroVacunoPdfReportResult(fileName, downloadUrl));
    }

    private Document CreateDocument(RegistroVacunoDetalle vacuno, CultureInfo culture)
    {
        var photoPath = GetBestPhotoPath(vacuno);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Lato));

                page.Header().Element(c => ComposeHeader(c, vacuno, culture));
                page.Content().Element(c => ComposeContent(c, vacuno, photoPath, culture));
                page.Footer().Element(ComposeFooter);
            });
        });
    }

    private void ComposeHeader(IContainer container, RegistroVacunoDetalle vacuno, CultureInfo culture)
    {
        container.Background(Colors.Blue.Darken3).Padding(20).Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("ZooTech").FontSize(32).Black().FontColor(Colors.White);
                column.Item().Text("Reporte Individual de Vacuno").FontSize(16).FontColor(Colors.Blue.Lighten4);
                column.Item().PaddingTop(10).Text($"Generado el: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", culture)}")
                      .FontSize(10).FontColor(Colors.Blue.Lighten4);
            });

            row.ConstantItem(150).AlignRight().Column(c => 
            {
                c.Item().Text("ID del Sistema").FontSize(10).FontColor(Colors.Blue.Lighten4).AlignRight();
                c.Item().Text($"#{vacuno.Id}").FontSize(24).Bold().FontColor(Colors.White).AlignRight();
            });
        });
    }

    private void ComposeContent(IContainer container, RegistroVacunoDetalle vacuno, string? photoPath, CultureInfo culture)
    {
        container.PaddingVertical(1, Unit.Centimetre).Column(column =>
        {
            column.Spacing(25);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(c => ComposeIdentificationSection(c, vacuno, culture));

                if (!string.IsNullOrEmpty(photoPath))
                {
                    row.ConstantItem(180).PaddingLeft(25).AlignRight()
                        .Width(155).Height(155)
                        .DefaultTextStyle(x => x.SemiBold())
                        .Border(2).BorderColor(Colors.Blue.Lighten2)
                        .Image(photoPath).FitArea();
                }
            });

            column.Item().Element(c => ComposeSection(c, "Trazabilidad", table =>
            {
                AddTableItem(table, "Código Padre", vacuno.CodigoPadre);
                AddTableItem(table, "Código Madre", vacuno.CodigoMadre);
                AddTableItem(table, "Código Abuelo", vacuno.CodigoAbuelo);
                AddTableItem(table, "Código Abuela", vacuno.CodigoAbuela);
                AddTableItem(table, "Granja", vacuno.Granja);
                AddTableItem(table, "Distrito", vacuno.Distrito);
                AddTableItem(table, "Provincia", vacuno.Provincia);
                AddTableItem(table, "Departamento", vacuno.Departamento);
                AddTableItem(table, "Procedencia", vacuno.Procedencia);
                AddTableItem(table, "Adquisición por", vacuno.AdquisicionPor);
                AddTableItem(table, "Precio compra", vacuno.PrecioCompra.HasValue ? vacuno.PrecioCompra.Value.ToString("0.##", culture) : "-");
                AddTableItem(table, "Fecha adquisición", vacuno.FechaAdquisicion.HasValue ? vacuno.FechaAdquisicion.Value.ToString("yyyy-MM-dd", culture) : "-");
            }));

            column.Item().Element(c => ComposeSection(c, "Especialización", table =>
            {
                AddTableItem(table, "Apto para", vacuno.AptoPara);
                AddTableItem(table, "Fecha especificación", vacuno.FechaEspecificacion.HasValue ? vacuno.FechaEspecificacion.Value.ToString("yyyy-MM-dd", culture) : "-");
            }));

            column.Item().Element(c => ComposeSection(c, "Observaciones y Estado", table =>
            {
                AddTableItem(table, "Observaciones", vacuno.Observaciones, 4);
                AddTableItem(table, "Motivo estado", vacuno.MotivoEstado, 4);
            }));

            column.Item().Element(c => ComposeSection(c, "Auditoría", table =>
            {
                AddTableItem(table, "Creado por", vacuno.CreadoPor);
                AddTableItem(table, "Creado en", vacuno.CreadoEn.ToString("yyyy-MM-dd HH:mm:ss", culture));
                AddTableItem(table, "Actualizado por", vacuno.ActualizadoPor);
                AddTableItem(table, "Actualizado en", vacuno.ActualizadoEn.ToString("yyyy-MM-dd HH:mm:ss", culture));
            }));
        });
    }

    private void ComposeIdentificationSection(ColumnDescriptor column, RegistroVacunoDetalle vacuno, CultureInfo culture)
    {
        ComposeSection(column.Item(), "Datos de Identificación", table =>
        {
            AddTableItem(table, "Código", vacuno.Codigo);
            AddTableItem(table, "Nombre", vacuno.Nombre);
            AddTableItem(table, "Fecha nacimiento", vacuno.FechaNacimiento.ToString("yyyy-MM-dd", culture));
            AddTableItem(table, "Sexo", vacuno.Sexo);
            AddTableItem(table, "Raza", vacuno.Raza);
            AddTableItem(table, "Color", vacuno.Color);
            AddTableItem(table, "Estado Actual", vacuno.Estado);
            AddTableItem(table, "Fecha registro", vacuno.FechaRegistro.ToString("yyyy-MM-dd", culture));
        });
    }

    private void ComposeSection(IContainer container, string title, Action<TableDescriptor> buildTable)
    {
        container.Column(column =>
        {
            column.Item().PaddingBottom(10).Row(row =>
            {
                row.AutoItem().Background(Colors.Blue.Darken2).PaddingVertical(4).PaddingHorizontal(10)
                   .Text(title).Bold().FontSize(14).FontColor(Colors.White);
                
                row.RelativeItem().PaddingTop(12).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);
            });

            column.Item().Background(Colors.Grey.Lighten4).Padding(15).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(120);
                    columns.RelativeColumn();
                    columns.ConstantColumn(120);
                    columns.RelativeColumn();
                });

                buildTable(table);
            });
        });
    }

    private void AddTableItem(TableDescriptor table, string label, string? value, uint columnsSpan = 1)
    {
        table.Cell().ColumnSpan(1).PaddingBottom(8).Text($"{label}:").SemiBold().FontColor(Colors.Grey.Darken3);
        table.Cell().ColumnSpan(columnsSpan).PaddingBottom(8).Text(string.IsNullOrWhiteSpace(value) ? "-" : value).FontColor(Colors.Black);
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text("ZooTech - Sistema Integral de Ganadería").FontSize(10).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("Página ").FontSize(10).FontColor(Colors.Grey.Medium);
                    x.CurrentPageNumber().FontSize(10).FontColor(Colors.Grey.Medium);
                    x.Span(" de ").FontSize(10).FontColor(Colors.Grey.Medium);
                    x.TotalPages().FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });
        });
    }

    private static string? GetBestPhotoPath(RegistroVacunoDetalle vacuno)
    {
        var candidates = new[]
        {
            vacuno.FotoUrl,
            vacuno.FotoRuta,
            vacuno.FotoNombreAlmacenado,
            vacuno.FotoNombreOriginal
        };

        var baseDirectory = AppContext.BaseDirectory;
        var wwwroot = Path.Combine(baseDirectory, "wwwroot");

        foreach (var candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate)) continue;

            if (Uri.TryCreate(candidate, UriKind.Absolute, out var uri) && uri.IsFile && File.Exists(uri.LocalPath))
                return uri.LocalPath;

            if (Uri.TryCreate(candidate, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                continue;

            if (Path.IsPathRooted(candidate) && File.Exists(candidate))
                return candidate;

            var relative = candidate.TrimStart('/', '\\');
            var inWwwroot = Path.Combine(wwwroot, relative);
            if (File.Exists(inWwwroot)) return inWwwroot;

            var inBase = Path.Combine(baseDirectory, relative);
            if (File.Exists(inBase)) return inBase;
        }

        return null;
    }

    private static string SanitizeFileNamePart(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "vacuno";
        var invalidChars = Path.GetInvalidFileNameChars();
        var safe = new string(value.Where(ch => !invalidChars.Contains(ch) && !char.IsWhiteSpace(ch)).ToArray());
        return string.IsNullOrWhiteSpace(safe) ? "vacuno" : safe;
    }
}

