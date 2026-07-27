using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.GetArbolGenealogico;
public interface IGetArbolGenealogicoInputPort
    : IRequestHandler<GetArbolGenealogicoQuery , GetArbolGenealogicoOutput>
{
}