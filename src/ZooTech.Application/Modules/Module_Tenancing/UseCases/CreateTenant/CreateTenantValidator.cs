using FluentValidation;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
    {
        public CreateTenantValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("El código es requerido.")
                .MaximumLength(50);

            RuleFor(x => x.SubDomain)
                .NotEmpty()
                .Matches("^[a-z0-9-]+$")
                .WithMessage("Subdominio inválido.");

            RuleFor(x => x.DisplayName)
                .NotEmpty()
                .MaximumLength(150)
                .WithMessage("El nombre visible para el tenant es requerido.");

            RuleFor(x => x.LegalName)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("El nombre legal o razón social es requerido.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Debe proporcionar un correo");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .MaximumLength(30)
                .WithMessage("El teléfono es requerido.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Estado inválido.");

            RuleFor(x => x.TenantAddress)
                .NotNull()
                .SetValidator(new TenantAddressValidator());

            RuleFor(x => x.TenantBranding)
                .NotNull()
                .SetValidator(new TenantBrandingValidator());

            RuleFor(x => x.TenantDatabaseConnection)
                .NotNull()
                .SetValidator(new TenantDatabaseConnectionValidator());
        }
    }

    public class TenantAddressValidator : AbstractValidator<TenantAddress>
    {
        public TenantAddressValidator()
        {
            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("El país es requerido.");

            RuleFor(x => x.State)
                .NotEmpty()
                .WithMessage("El estado o departamento es requerido.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("La ciudad es requerida.");

            RuleFor(x => x.AddressLine_1)
                .NotEmpty()
                .MaximumLength(300)
                .WithMessage("Debe proporcionar al menos una dirección.");
        }
    }

    public class TenantBrandingValidator : AbstractValidator<TenantBranding>
    {
        public TenantBrandingValidator()
        {
            RuleFor(x => x.PrimaryColor)
                .Matches("^#([A-Fa-f0-9]{6})$")
                .WithMessage("Color hexadecimal inválido.");

            RuleFor(x => x.SecondaryColor)
                .Matches("^#([A-Fa-f0-9]{6})$");

            RuleFor(x => x.LogoUrl)
                .Must(url =>
                    string.IsNullOrWhiteSpace(url)
                    || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("LogoUrl inválido.");
        }
    }

    public class TenantDatabaseConnectionValidator : AbstractValidator<TenantDatabaseConnection>
    {
        public TenantDatabaseConnectionValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("Debe especificar el estado de la base de datos.");
        }
    }
}
