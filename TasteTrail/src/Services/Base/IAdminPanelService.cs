using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Services.Base;

public interface IAdminPanelService
{
    Task<int> GetUsersCountAsync(FilterType type = FilterType.NoFilter, int pageNumber = 1, int pageSize = 10);
    Task<bool> AssignRoleAsync(string userId, UserRoles role);
    Task<bool> RemoveRoleAsync(string userId, UserRoles role);
    Task<bool> ToggleMuteAsync(string userId);
    Task<bool> ToggleBanAsync(string userId);
    Task<UserDto> GetUserByIdAsync(string userId);
    Task<UserListDto?> GetAllUsersAsync(FilterType type = FilterType.NoFilter, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
}
