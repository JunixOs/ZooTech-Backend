using System;
using FluentValidation;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.UpdateVacuno;

namespace ZooTech.Application.Modules.Module_Vacuno.Validators;

internal sealed class UpdateVacunoValidator : AbstractValidator<UpdateVacunoCommand>
{
    public UpdateVacunoValidator()
    {
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
