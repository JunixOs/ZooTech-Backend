using System;
using FluentValidation;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.CreateVacuno;

namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

internal sealed class CreateVacunoValidator : AbstractValidator<CreateVacunoCommand>
{
    public CreateVacunoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("El código del vacuno es obligatorio.")
            .MaximumLength(10).WithMessage("El código no puede superar los 10 caracteres.")
            .Matches("^[A-Z0-9]+$").WithMessage("El código debe estar en mayúsculas y contener solo letras y números.");

        RuleFor(x => x.Codigo)
            .Matches("^VAC[0-9]+$").WithMessage("El codigo debe iniciar con VAC y continuar solo con numeros.");

        RuleFor(x => x.Nombre).VacunoNombreRules();
        RuleFor(x => x.FechaNacimiento).VacunoFechaNacimientoRules();
        RuleFor(x => x.TipoAdquisicionCode).VacunoTipoAdquisicionCodeRules();
        RuleFor(x => x.RazaCode).VacunoRazaCodeRules();
        RuleFor(x => x.ColorCode).VacunoColorCodeRules();
        RuleFor(x => x.SexoCode).VacunoSexoCodeRules();
        RuleFor(x => x.GranjaId).VacunoGranjaIdRules();
        RuleFor(x => x.Observaciones).VacunoObservacionesRules(x => x.Observaciones is not null);
        RuleFor(x => x.PrecioCompra).VacunoPrecioCompraRules(x => x.TipoAdquisicionCode == "COMPRA");
    }
}
