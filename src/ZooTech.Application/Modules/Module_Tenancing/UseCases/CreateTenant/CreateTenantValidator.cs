using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant
{
    public class CreateTenantValidation : ICommandValidator<CreateTenantCommand>
    {
        public ModuleName ModuleName => ModuleName.Tenancing;

        public List<string> Validate(CreateTenantCommand command)
        {
            var errors = new List<string>();

            // Tenant
            ValidateRequired(command.Code, "TENANT-CODE", errors, 50);
            ValidateSubDomain(command.SubDomain, errors);
            ValidateRequired(command.DisplayName, "TENANT-DISPLAY_NAME", errors, 150);
            ValidateRequired(command.LegalName, "TENANT-LEGAL_NAME", errors, 200);
            ValidateEmail(command.Email, errors);
            ValidateRequired(command.Phone, "TENANT-PHONE", errors, 30);

            if (!Enum.IsDefined(command.Status))
                errors.Add("TENANCING_CREATE-TENANT-STATUS-INVALID");

            // Address
            if (command.TenantAddress == null)
            {
                errors.Add("TENANCING_CREATE-TENANT_ADDRESS-NULL");
            }
            else
            {
                ValidateAddress(command.TenantAddress, errors);
            }

            // Branding
            if (command.TenantBranding == null)
            {
                errors.Add("TENANCING_CREATE-TENANT_BRANDING-NULL");
            }
            else
            {
                ValidateBranding(command.TenantBranding, errors);
            }

            // Database
            if (command.TenantDatabaseConnection == null)
            {
                errors.Add("TENANCING_CREATE-TENANT_DATABASE_CONNECTION-NULL");
            }

            return errors;
        }

        private static void ValidateAddress(TenantAddress address, List<string> errors)
        {
            ValidateRequired(address.Country, "TENANT_ADDRESS-ADDRESS_COUNTRY", errors);
            ValidateRequired(address.State, "TENANT_ADDRESS-STATE", errors);
            ValidateRequired(address.City, "TENANT_ADDRESS-CITY", errors);
            ValidateRequired(address.AddressLine_1, "TENANT_ADDRESS-ADDRESS_LINE_1", errors, 300);
        }

        private static void ValidateBranding(TenantBranding branding, List<string> errors)
        {
            ValidateHexColor(branding.PrimaryColor, "TENANT_BRANDING-PRIMARY_COLOR", errors);
            ValidateHexColor(branding.SecondaryColor, "TENANT_BRANDING-SECONDARY_COLOR", errors);

            if (!string.IsNullOrWhiteSpace(branding.LogoUrl) &&
                !Uri.IsWellFormedUriString(branding.LogoUrl, UriKind.Absolute))
            {
                errors.Add("TENANCING_CREATE-TENANT_BRANDING-LOGO_URL-INVALID");
            }
        }

        private static void ValidateRequired(string? value, string field, List<string> errors, int? maxLength = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"TENANCING_CREATE-{field}-NULL");
                return;
            }

            if (maxLength.HasValue && value.Length > maxLength.Value)
            {
                errors.Add($"TENANCING_CREATE-{field}-INVALID");
            }
        }

        private static void ValidateEmail(string? email, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("TENANCING_CREATE-TENANT-EMAIL-NULL");
                return;
            }

            try
            {
                _ = new System.Net.Mail.MailAddress(email);
            }
            catch
            {
                errors.Add("TENANCING_CREATE-TENANT-EMAIL-INVALID");
            }
        }

        private static void ValidateSubDomain(string? subDomain, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(subDomain))
            {
                errors.Add("TENANCING_CREATE-TENANT-SUBDOMAIN-NULL");
                return;
            }

            foreach (char c in subDomain)
            {
                if (!(char.IsLower(c) || char.IsDigit(c) || c == '-'))
                {
                    errors.Add("TENANCING_CREATE-TENANT-SUBDOMAIN-INVALID");
                    return;
                }
            }
        }

        private static void ValidateHexColor(string? color, string field, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(color))
                return;

            if (color.Length != 7 || color[0] != '#')
            {
                errors.Add($"TENANCING_CREATE-{field}-INVALID");
                return;
            }

            for (int i = 1; i < color.Length; i++)
            {
                if (!Uri.IsHexDigit(color[i]))
                {
                    errors.Add($"TENANCING_CREATE-{field}-INVALID");
                    return;
                }
            }
        }
    }
}