using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Filters;

namespace ZooTech.InterfaceAdapters.Modules.Module_GestionUsuarios.Controllers
{
    [ApiController]
    [Route("api/v1/gestionusuarios/admin-user")]
    // El nombre GroupName debe coincidir con el definido en options.SwaggerDoc() y options.Endpoint() en ZooTech.Api/Program.cs
    [ApiExplorerSettings(GroupName = "auth")]
    public class AdminUserController : ControllerBase
    {
        private readonly IBehaviorDispatcher _behaviorDispatcher;

        public AdminUserController(
            IBehaviorDispatcher behaviorDispatcher
        )
        {
            _behaviorDispatcher = behaviorDispatcher;
        }
    }
}
