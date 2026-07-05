// TODO: Añadir Crear, Modificar y Eliminar Admin User
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.CreateAdminUser;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.DeleteAdminUser;
using ZooTech.Application.Common.Behaviors.Module_Tenancing.ListAdminUsers;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.DeleteAdminUser;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Controllers
{
    [ApiController]
    [Route("api/v1/admin-user")]
    public class AdminUserController : ControllerBase
    {
        private readonly IListAdminUsersBehaviorPipelineFactory _listAdminUsersBehaviorPipelineFactory;
        private readonly IDeleteAdminUserBehaviorPipelineFactory _deleteAdminUserBehaviorPipelineFactory;
        private readonly ICreateAdminUserBehaviorPipelineFactory _createAdminUserBehaviorPipelineFactory;

        public AdminUserController(
            IListAdminUsersBehaviorPipelineFactory listAdminUsersBehaviorPipelineFactory,
            IDeleteAdminUserBehaviorPipelineFactory deleteAdminUserBehaviorPipelineFactory,
            ICreateAdminUserBehaviorPipelineFactory createAdminUserBehaviorPipelineFactory
        )
        {
            _listAdminUsersBehaviorPipelineFactory = listAdminUsersBehaviorPipelineFactory;
            _deleteAdminUserBehaviorPipelineFactory = deleteAdminUserBehaviorPipelineFactory;
            _createAdminUserBehaviorPipelineFactory = createAdminUserBehaviorPipelineFactory;
        }

        // TODO: Aplicar Guia de Paginacion
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpGet("users")]
        public async Task<IActionResult> ListAdminUsers()
        {
            var behaviorPipeline = _listAdminUsersBehaviorPipelineFactory.Create();

            var result = await behaviorPipeline.Execute(
                EmptyCommand.Value
            );

            return Ok(ListAdminUsersMapper.ToResponse(result));
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpGet("delete")]
        public async Task<IActionResult> DeleteAdminUser(
            [FromQuery] int? id
        )
        {
            var behaviorPipeline = _deleteAdminUserBehaviorPipelineFactory.Create();

            await behaviorPipeline.Execute(
                new DeleteAdminUserCommand
                {
                    Id = id
                }
            );

            return Ok();
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAdminUser(
            [FromBody] CreateAdminUserRequestDTO requestDto
        )
        {
            var behaviorPipeline = _createAdminUserBehaviorPipelineFactory.Create();

            var result = await behaviorPipeline.Execute(
                CreateAdminUserMapper.ToCommand(requestDto)
            );

            return Ok(CreateAdminUserMapper.ToResponseDto(result));
        }
    }
}