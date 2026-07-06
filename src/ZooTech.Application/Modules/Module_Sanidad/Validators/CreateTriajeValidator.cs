using FluentValidation;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.CreateTriaje;

namespace ZooTech.Application.Modules.Module_Sanidad.Validators;

internal sealed class CreateTriajeValidator : AbstractValidator<CreateTriajeCommand>
{
    public CreateTriajeValidator()
    {
        RuleFor(x => x.VacunoId).TriajeVacunoIdRules();
        RuleFor(x => x.TipoPesoCode).TriajeTipoPesoCodeRules();
        RuleFor(x => x.PesoKg).TriajePesoKgRules();
        RuleFor(x => x.Observaciones).TriajeObservacionesRules(x => x.Observaciones is not null);
        RuleFor(x => x.FechaHora).TriajeFechaHoraRules();
    }
}
