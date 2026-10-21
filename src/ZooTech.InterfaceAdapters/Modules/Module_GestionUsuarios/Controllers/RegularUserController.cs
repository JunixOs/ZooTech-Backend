using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios.DTOs;
using ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios
{
    [ApiController]
    [Route("api/v1/gestionusuarios/reg-user")]
    [ApiExplorerSettings(GroupName = "auth")]
    public class RegularUserController : ControllerBase
    {
        private readonly IBehaviorDispatcher _behaviorDispatcher;

        public RegularUserController(
            IBehaviorDispatcher behaviorDispatcher
        )
        {
            _behaviorDispatcher = behaviorDispatcher;
        }
    }
}
