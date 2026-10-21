// TODO: Añadir Crear, Modificar y Eliminar Admin User
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZooTech.Application.Common.Behaviors;
using ZooTech.Application.Common.Models;
using ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.CreateAdminUser;
using ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.DeleteAdminUser;
using ZooTech.Application.Modules.Module_Administracion.UseCases.AdminUserManagement.ListAdminUsers;
using ZooTech.Domain.Shared.Enums;
using ZooTech.InterfaceAdapters.Filters;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Administracion.Mappers;

namespace ZooTech.InterfaceAdapters.Modules.Module_Administracion.Controllers
{
    [ApiController]
    [Route("api/v1/administracion/admin-user")]
    [ApiExplorerSettings(GroupName = "administracion")]
    public class AdminUserManagementController : ControllerBase
    {
        private readonly IBehaviorDispatcher _behaviorDispatcher;
        public AdminUserManagementController(
            IBehaviorDispatcher behaviorDispatcher
        )
        {
            _behaviorDispatcher = behaviorDispatcher;
        }

        // TODO: Aplicar Guia de Paginacion
        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpGet("users")]
        [Tags("Admin User Management")]
        public async Task<IActionResult> ListAdminUsers(CancellationToken cancellationToken = default)
        {
            var result = await _behaviorDispatcher.Send<ListAdminUsersQuery , List<ListAdminUsersOutput>>(
                new ListAdminUsersQuery(),
                cancellationToken
            );

            return Ok(ListAdminUsersMapper.ToResponse(result));
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpGet("delete")]
        [Tags("Admin User Management")]
        public async Task<IActionResult> DeleteAdminUser(
            [FromQuery] int? id,
            CancellationToken cancellationToken = default
        )
        {
            await _behaviorDispatcher.Send<DeleteAdminUserCommand , EmptyOutput>(
                new DeleteAdminUserCommand
                {
                    Id = id
                },
                cancellationToken
            );

            return Ok();
        }

        [ServiceFilter(typeof(TenantHeaderFilter))]
        [RestrictTenantType(TenantType.Admin)]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        [HttpPost]
        [Tags("Admin User Management")]
        public async Task<IActionResult> CreateAdminUser(
            [FromBody] CreateAdminUserRequestDTO requestDto,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _behaviorDispatcher.Send<CreateAdminUserCommand , CreateAdminUserOutput>(
                CreateAdminUserMapper.ToCommand(requestDto),
                cancellationToken
            );

            return Ok(CreateAdminUserMapper.ToResponseDto(result));
        }
    }
}