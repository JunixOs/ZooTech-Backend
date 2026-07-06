using ZooTech.Application.Common.Pagination;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetCelos;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListCelos;

public sealed record ListCelosOutput(PagedResult<CeloItemDto> Result);
