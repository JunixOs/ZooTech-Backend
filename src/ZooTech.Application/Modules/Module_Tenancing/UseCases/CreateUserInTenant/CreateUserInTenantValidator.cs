using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Tenancing.Validators;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateUserInTenant
{
    public class CreateUserInTenantValidator : ICommandValidator<CreateUserInTenantCommand>
    {
        public ModuleName ModuleName => ModuleName.Tenancing;

        public List<string> Validate(CreateUserInTenantCommand request)
        {
            var errors = new List<string>();

            TenancingCommonValidationRules.ValidateEmailAndUsername(errors, "CREATE", request.Email, request.UserName);

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