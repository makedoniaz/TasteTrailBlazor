using TasteTrailBlazor.Dtos;
using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Services.Base;

public interface IMenuService
{
    Task<MenuDto> GetFilteredMenusAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = "");
    Task<MenuDto> GetFilteredMenusAsync(int venueId, int pageNumber = 1, int pageSize = 10, string searchTerm = "");
    Task<Menu?> GetMenuByIdAsync(int id);
    Task<List<Menu>?> GetMenusFromToAsync(int from, int to, int venueId);
    Task<bool> CreateMenuAsync(MenuCreateDto menuDto, Stream imageStream, string logoFileName);
    Task<bool> UpdateMenuAsync(MenuUpdateDto menuDto, Stream imageStream, string logoFileName);
    Task<bool> DeleteMenuAsync(int id);
}
