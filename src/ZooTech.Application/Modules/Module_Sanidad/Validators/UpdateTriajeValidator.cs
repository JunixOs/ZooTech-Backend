using FluentValidation;
using ZooTech.Application.Modules.Module_Sanidad.UseCases.UpdateTriaje;

namespace ZooTech.Application.Modules.Module_Sanidad.Validators;

internal sealed class UpdateTriajeValidator : AbstractValidator<UpdateTriajeCommand>
{
    public UpdateTriajeValidator()
    {
        RuleFor(x => x.VacunoId).TriajeVacunoIdRules();
        RuleFor(x => x.TipoPesoCode).TriajeTipoPesoCodeRules();
        RuleFor(x => x.PesoKg).TriajePesoKgRules();
        RuleFor(x => x.EstadoRegistroCode).TriajeEstadoRegistroCodeRules();
        RuleFor(x => x.Observaciones).TriajeObservacionesRules(x => x.Observaciones is not null);
    }
}
