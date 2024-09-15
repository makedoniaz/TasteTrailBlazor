using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Services.Base;

public interface IMenuItemService
{
    Task<MenuItem?> GetMenuItemByIdAsync(int id);
    Task<MenuItemDto> GetFilteredMenuItemsAsync(FilterType type = FilterType.NoFilter, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
    Task<MenuItemDto> GetFilteredMenuItemsAsync(int menuId, FilterType type = FilterType.NoFilter, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
    Task<bool> CreateMenuItemAsync(MenuItemCreateDto menuItemDto, Stream imageStream, string imageFileName);
    Task<bool> UpdateMenuItemAsync(MenuItemUpdateDto menuItemDto, Stream imageStream, string imageFileName);
    Task<bool> DeleteMenuItemAsync(int id);
}
