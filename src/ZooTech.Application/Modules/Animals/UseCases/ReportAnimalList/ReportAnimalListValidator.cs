namespace ZooTech.Application.Modules.Animals.UseCases.ReportAnimalList;

public sealed class ReportAnimalListValidator
{
    private const int DefaultDays = 30;
    private const int MaxRangeDays = 366;
    private const int MaximumKeywordLength = 100;

    public ReportAnimalListFilter ValidateAndNormalize(ReportAnimalListCommand command)
    {
        Dictionary<string, string> errors = [];

        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var fechaFin = command.FechaFin ?? today;
        var fechaInicio = command.FechaInicio ?? fechaFin.AddDays(-DefaultDays);
        var keyword = string.IsNullOrWhiteSpace(command.Keyword)
            ? null
            : command.Keyword.Trim();
        var razaCode = NormalizeCode(command.RazaCode);
        var colorCode = NormalizeCode(command.ColorCode);
        var sexoCode = NormalizeCode(command.SexoCode);
        var tipoAdquisicionCode = NormalizeCode(command.TipoAdquisicionCode);
        var estadoCode = NormalizeCode(command.EstadoCode);

        if (command.FechaInicio.HasValue && !command.FechaFin.HasValue)
        {
            fechaFin = today;
        }

        if (!command.FechaInicio.HasValue && command.FechaFin.HasValue)
        {
            fechaInicio = fechaFin.AddDays(-DefaultDays);
        }

        if (fechaInicio > fechaFin)
        {
            errors["fechaInicio"] = "La fecha de inicio no puede ser mayor a la fecha de fin.";
        }

        if (fechaFin.DayNumber - fechaInicio.DayNumber > MaxRangeDays)
        {
            errors["rangoFechas"] = $"El rango del reporte no debe superar {MaxRangeDays} dias.";
        }

        if (keyword?.Length > MaximumKeywordLength)
        {
            errors["keyword"] = $"El keyword no debe superar {MaximumKeywordLength} caracteres.";
        }

        if (command.GranjaId.HasValue && command.GranjaId.Value <= 0)
        {
            errors["granjaId"] = "El identificador de granja debe ser mayor a cero.";
        }

        if (errors.Count > 0)
        {
            throw new ReportAnimalListValidationException(errors);
        }

        return new ReportAnimalListFilter(
            fechaInicio,
            fechaFin,
            keyword,
            razaCode,
            colorCode,
            sexoCode,
            tipoAdquisicionCode,
            command.GranjaId,
            estadoCode);
    }

    private static string? NormalizeCode(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
