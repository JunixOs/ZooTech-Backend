using Microsoft.AspNetCore.Mvc;
using ZooTech.InterfaceAdapters.Filters;

namespace ZooTech.InterfaceAdapters.Modules.Module_Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    [ApiExplorerSettings(GroupName = "auth")]
    public class AuthController : ControllerBase
    {
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
        {
            return Ok();
        }
    }
}