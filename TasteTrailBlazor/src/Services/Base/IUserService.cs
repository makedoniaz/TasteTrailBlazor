using TasteTrailBlazor.Dtos;

namespace TasteTrailBlazor.Services.Base;

public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserAsync(string userId, UserDto userDto);
        Task<UserDto?> GetUserRolesAsync(string userId);
    }