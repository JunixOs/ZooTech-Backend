using FluentValidation;
using ZooTech.Application.Common.Gateway.Time;
using ZooTech.Domain.Module_Reproduccion.Entities;

namespace ZooTech.Application.Modules.Module_Reproduccion.UseCases.RegistrarFecundacion;

public sealed class RegistrarFecundacionValidator : AbstractValidator<RegistrarFecundacionCommand>
{
    public RegistrarFecundacionValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.TipoFecundacionCode)
            .NotEmpty().WithMessage("El tipo de fecundación es requerido.")
            .Must(x => x == Fecundacion.TipoMontaNatural || x == Fecundacion.TipoInseminacion || x == Fecundacion.TipoTransferencia)
            .WithMessage("El tipo de fecundación no es válido.");

        RuleFor(x => x.VacunoReceptorId)
            .GreaterThan(0).WithMessage("El receptor es requerido.");

        RuleFor(x => x.FechaProcedimiento)
            .NotEmpty().WithMessage("La fecha del procedimiento es requerida.")
            .LessThanOrEqualTo(x => DateOnly.FromDateTime(dateTimeProvider.ServerNow))
            .WithMessage("La fecha del procedimiento no puede ser futura.");

        RuleFor(x => x.ResponsableId)
            .GreaterThan(0).WithMessage("El responsable es requerido.");

        When(x => x.TipoFecundacionCode == Fecundacion.TipoInseminacion, () =>
        {
            RuleFor(x => x.CodigoSemen)
                .NotEmpty().WithMessage("El código de semen es requerido para Inseminación Artificial.");
        });

        When(x => x.TipoFecundacionCode == Fecundacion.TipoTransferencia, () =>
        {
            RuleFor(x => x.CodigoEmbrion)
                .NotEmpty().WithMessage("El código de embrión es requerido para Transferencia de Embriones.");
        });

        RuleFor(x => x)
            .Must(x => x.VacunoDonanteId.HasValue || !string.IsNullOrWhiteSpace(x.NombreMachoExterno))
            .WithMessage("Debe especificar un donante interno o el nombre de un macho externo.");
    }
}
