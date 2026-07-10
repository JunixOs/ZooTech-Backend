using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser
{
    public class DeleteAdminUserValidator : ICommandValidator<DeleteAdminUserCommand>
    {
        public ModuleName ModuleName => ModuleName.Tenancing;

        public List<string> Validate(DeleteAdminUserCommand cmd)
        {
            var errors = new List<string>();

            if (cmd.Id is null)
            {
                errors.Add("TENANCING-ADMIN_USER-DELETE-ID-NULL");
            }

            return errors;
        }
    }
}