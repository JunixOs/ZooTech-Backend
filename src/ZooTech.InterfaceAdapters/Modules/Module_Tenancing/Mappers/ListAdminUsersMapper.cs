using ZooTech.Application.Modules.Module_Tenancing.UseCases.ListAdminUsers;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Mappers
{
    public class ListAdminUsersMapper
    {
        public static List<ListAdminUsersResponseDTO> ToResponse(List<ListAdminUsersOutput> outputList)
        {
            return outputList.Select(ol => new ListAdminUsersResponseDTO
            {
                Id = ol.Id,
                Email = ol.Email,
                Username = ol.Email,
                FirstName = ol.FirstName,
                IsActive = ol.IsActive,
                LastLoginAt = ol.LastLoginAt,
                CreatedAt = ol.CreatedAt
                
            })
            .ToList();
        }
    }
}