using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Services.Base;

public interface IMenuItemService
{
    Task<MenuItem?> GetMenuItemByIdAsync(int id);
    Task<MenuItemDto> GetFilteredMenuItemsAsync(FilterType type, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
    Task<MenuItemDto> GetFilteredMenuItemsAsync(FilterType type, int menuId, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
    Task CreateMenuItemAsync(MenuItemDto menuItemDto);
    Task UpdateMenuItemAsync(MenuItemDto menuItemDto);
    Task DeleteMenuItemAsync(int id);
}
