using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.DeleteCelo
{
    public class DeleteCeloValidator : ICommandValidator<DeleteCeloCommand>
    {
        public ModuleName ModuleName => ModuleName.Celo;

        public List<string> Validate(DeleteCeloCommand request)
        {
            var errors = new List<string>();

            if (request.Id <= 0)
            {
                errors.Add("CELO-CELO-DELETE-ID-INVALID");
            }

            if (string.IsNullOrWhiteSpace(request.MotivoEliminacion))
            {
                errors.Add("CELO-CELO-DELETE-MOTIVO_ELIMINACION-NULL");
            }
            else if (request.MotivoEliminacion.Length > 500)
            {
                errors.Add("CELO-CELO-DELETE-MOTIVO_ELIMINACION-INVALID");
            }

            return errors;
        }
    }
}