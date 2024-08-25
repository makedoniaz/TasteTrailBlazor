using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Services.Base;

public interface IMenuItemService
{
    Task<MenuItem?> GetMenuItemByIdAsync(int id);
    Task<List<MenuItem>?> GetMenuItemsFromToAsync(int from, int to, int menuId);
    Task CreateMenuItemAsync(MenuItemDto menuItemDto);
    Task UpdateMenuItemAsync(MenuItemDto menuItemDto);
    Task DeleteMenuItemAsync(int id);
}
