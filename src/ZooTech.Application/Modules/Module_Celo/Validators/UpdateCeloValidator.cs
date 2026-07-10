using ZooTech.Application.Common.Validator;
using ZooTech.Application.Modules.Module_Celo.UseCases.UpdateCelo;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.Validators
{
    public class UpdateCeloValidator : ICommandValidator<UpdateCeloCommand>
    {
        public ModuleName ModuleName => ModuleName.Celo;

        public List<string> Validate(UpdateCeloCommand request)
        {
            var errors = new List<string>();

            if (request.Id <= 0)
            {
                errors.Add("CELO-CELO-UPDATE-ID-INVALID");
            }

            if (!string.IsNullOrWhiteSpace(request.Observaciones) &&
                request.Observaciones.Length > 500)
            {
                errors.Add("CELO-CELO-UPDATE-OBSERVACIONES-INVALID");
            }

            return errors;
        }
    }
}