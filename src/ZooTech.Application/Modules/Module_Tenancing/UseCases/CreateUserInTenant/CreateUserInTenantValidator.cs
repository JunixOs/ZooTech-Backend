using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant
{
    public class CreateUserInTenantValidator : ICommandValidator<CreateUserInTenantCommand>
    {
        public ModuleName ModuleName => ModuleName.Tenancing;

        public List<string> Validate(CreateUserInTenantCommand request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-EMAIL-NULL");
            }
            else if (!System.Net.Mail.MailAddress.TryCreate(request.Email, out _))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-");
            }

            if (string.IsNullOrWhiteSpace(request.UserName))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-USERNAME-NULL");
            }
            else if (request.UserName.Length < 3 || request.UserName.Length > 50)
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-USERNAME-INVALID");
            }

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-FULL_NAME-NULL");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-CODE-NULL");
            } 
            else if (request.Code.Length > 15) 
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-CODE-INVALID");
            }

            if (string.IsNullOrWhiteSpace(request.TenantDatabaseName))
            {
                errors.Add("TENANCING-ADMIN_USER-CREATE-TENANT_DATABASE_NAME-NULL");
            }

            return errors;
        }
    }
}