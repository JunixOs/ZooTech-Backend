using FluentValidation;
using ZooTech.Application.Modules.Module_Fecundacion.UseCases.CreateFecundacion;
using ZooTech.Domain.Module_Fecundacion.Rules;

namespace ZooTech.Application.Modules.Module_Fecundacion.Validators;

public sealed class CreateFecundacionValidator : AbstractValidator<CreateFecundacionCommand>
{
    public CreateFecundacionValidator()
    {
        RuleFor(x => x.VacunoReceptorId)
            .GreaterThan(0).WithMessage("El ID del vacuno receptor debe ser mayor que cero.");

        RuleFor(x => x.TipoFecundacionCode)
            .NotEmpty().WithMessage("El tipo de fecundación es obligatorio.");

        RuleFor(x => x.ResultadoCode)
            .NotEmpty().WithMessage("El resultado de la fecundación es obligatorio.");

        RuleFor(x => x.ResponsableName)
            .NotEmpty().WithMessage("El nombre del responsable es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre del responsable no puede superar los 100 caracteres.");

        RuleFor(x => x.ObservacionesVeterinarias)
            .MaximumLength(250).When(x => x.ObservacionesVeterinarias is not null)
            .WithMessage("Las observaciones veterinarias no pueden superar los 250 caracteres.");

        RuleFor(x => x.FechaProcedimiento)
            .NotEmpty().WithMessage("La fecha del procedimiento es obligatoria.");

        RuleFor(x => x.MachoExternoNombre)
            .NotEmpty().When(x => x.MachoExterno)
            .WithMessage("El nombre del macho externo es obligatorio si se indica que es macho externo.");

        RuleFor(x => x.VacunoDonanteId)
            .NotNull().When(x => !x.MachoExterno)
            .WithMessage("Debe seleccionar un vacuno donante si no es macho externo.")
            .GreaterThan(0).When(x => !x.MachoExterno && x.VacunoDonanteId.HasValue)
            .WithMessage("El ID del vacuno donante debe ser mayor que cero.");

        When(x => FecundacionRules.EsInseminacionArtificial(x.TipoFecundacionCode), () =>
        {
            RuleFor(x => x.CodigoSemen)
                .NotEmpty()
                .WithMessage("El codigo de semen es obligatorio para inseminacion artificial.")
                .MaximumLength(30)
                .WithMessage("El codigo de semen no puede superar los 30 caracteres.");
        });

        When(x => FecundacionRules.EsTransferenciaEmbriones(x.TipoFecundacionCode), () =>
        {
            RuleFor(x => x.CodigoEmbrion)
                .NotEmpty()
                .WithMessage("El codigo de embrion es obligatorio para transferencia de embriones.")
                .MaximumLength(30)
                .WithMessage("El codigo de embrion no puede superar los 30 caracteres.");
        });
    }
}
