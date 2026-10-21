using System.Text.RegularExpressions;
using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_GestionUsuarios.UseCases.RegularUser.Login
{
    public class LoginValidator : ICommandQueryValidator<LoginCommand>
    {
        public ModuleName ModuleName => ModuleName.Auth;

        public List<string> Validate(LoginCommand cmd)
        {
            List<string> errors = new List<string>();

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (cmd.Email is null)
            {
                errors.Add("AUTH_LOGIN_EMAIL_NULL");
            }
            else if (!Regex.IsMatch(cmd.Email , emailPattern))
            {
                errors.Add("AUTH_LOGIN_EMAIL_INVALID_FORMAT");
            }

            return errors;
        }
    }
}