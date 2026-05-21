using FluentValidation;
using ZooTech.Domain.Module_Vacuno.Rules;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.RegistrarVacuno;

/// <summary>
/// Validaciones de entrada para el comando RegistrarVacuno.
/// Se ejecutan antes del Handler gracias al ValidationBehavior de MediatR pipeline.
/// </summary>
public sealed class RegistrarVacunoValidator : AbstractValidator<RegistrarVacunoCommand>
{
    private static readonly string[] FormatosImagenPermitidos = { ".png", ".jpg", ".jpeg" };

    public RegistrarVacunoValidator()
    {
        // ── Código ────────────────────────────────────────────────────
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código es obligatorio.")
            .MaximumLength(VacunoCodigoRule.LongitudMaxima)
                .WithMessage($"El código no puede exceder {VacunoCodigoRule.LongitudMaxima} caracteres.")
            .Matches("^[A-Z0-9]+$")
                .WithMessage("El código debe estar en mayúsculas y no contener caracteres especiales.");

        // ── Nombre ────────────────────────────────────────────────────
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(15).WithMessage("El nombre no puede exceder 15 caracteres.");

        // ── Fecha de nacimiento ───────────────────────────────────────
        RuleFor(x => x.FechaNacimiento)
            .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("La fecha de nacimiento no puede ser futura.");

        // ── Tipo de adquisición ───────────────────────────────────────
        RuleFor(x => x.IdTipoAdquisicion)
            .GreaterThan(0).WithMessage("El tipo de adquisición es obligatorio.");

        // ── Precio de compra (condicional) ────────────────────────────
        // ID 2 = Compra según cat_tipo_adquisicion
        RuleFor(x => x.PrecioCompra)
            .NotNull().WithMessage("El precio de compra es obligatorio cuando la adquisición es por compra.")
            .GreaterThan(0).WithMessage("El precio de compra debe ser mayor a 0.")
            .When(x => x.IdTipoAdquisicion == 2);

        RuleFor(x => x.PrecioCompra)
            .Null().WithMessage("El precio de compra solo aplica cuando la adquisición es por compra.")
            .When(x => x.IdTipoAdquisicion != 2);

        // ── Características ───────────────────────────────────────────
        RuleFor(x => x.IdRaza)
            .GreaterThan(0).WithMessage("La raza es obligatoria.");

        RuleFor(x => x.IdColor)
            .GreaterThan(0).WithMessage("El color es obligatorio.");

        RuleFor(x => x.IdSexo)
            .GreaterThan(0).WithMessage("El sexo es obligatorio.");

        // ── Trazabilidad ──────────────────────────────────────────────
        RuleFor(x => x.CodigoPadre)
            .NotEmpty().WithMessage("El código del padre es obligatorio.")
            .MaximumLength(10).WithMessage("El código del padre no puede exceder 10 caracteres.")
            .Matches("^[A-Z0-9]+$").WithMessage("El código del padre debe estar en mayúsculas.");

        RuleFor(x => x.CodigoMadre)
            .NotEmpty().WithMessage("El código de la madre es obligatoria.")
            .MaximumLength(10).WithMessage("El código de la madre no puede exceder 10 caracteres.")
            .Matches("^[A-Z0-9]+$").WithMessage("El código de la madre debe estar en mayúsculas.");

        // ── Procedencia ───────────────────────────────────────────────
        RuleFor(x => x.NombreGranja)
            .NotEmpty().WithMessage("La granja es obligatoria.")
            .MaximumLength(15).WithMessage("El nombre de la granja no puede exceder 15 caracteres.");

        RuleFor(x => x.IdDistrito)
            .GreaterThan(0).WithMessage("El distrito es obligatorio.");

        RuleFor(x => x.IdDepartamento)
            .GreaterThan(0).WithMessage("El departamento es obligatorio.");

        RuleFor(x => x.IdProvincia)
            .GreaterThan(0).WithMessage("La provincia es obligatoria.");

        // ── Utilización ───────────────────────────────────────────────
        RuleFor(x => x.IdTipoUtilizacion)
            .GreaterThan(0).WithMessage("El tipo de utilización (apto para) es obligatorio.");

        RuleFor(x => x.FechaEspecificacion)
            .NotEmpty().WithMessage("La fecha de especificación es obligatoria.");

        // ── Observaciones ─────────────────────────────────────────────
        RuleFor(x => x.Observaciones)
            .MaximumLength(VacunoObservacionesRule.MaxCaracteres)
                .WithMessage($"Las observaciones no pueden exceder {VacunoObservacionesRule.MaxCaracteres} caracteres.")
            .Must(obs => VacunoObservacionesRule.EsValido(obs))
                .WithMessage($"Las observaciones no pueden exceder {VacunoObservacionesRule.MaxPalabras} palabras.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observaciones));

        // ── Foto (opcional) ───────────────────────────────────────────
        RuleFor(x => x.FotoNombreOriginal)
            .Must(nombre =>
            {
                if (string.IsNullOrWhiteSpace(nombre)) return true;
                var ext = Path.GetExtension(nombre).ToLowerInvariant();
                return FormatosImagenPermitidos.Contains(ext);
            })
            .WithMessage("La foto debe ser PNG, JPG o JPEG.")
            .When(x => x.FotoStream is not null);
    }
}