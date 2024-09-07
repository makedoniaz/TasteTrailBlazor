using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Dtos;
 
public class MenuItemDto
{
    public int CurrentPage { get; set; }
    public int AmountOfPages { get; set; }
    public int AmountOfEntities { get; set; }
    public List<MenuItem> Entities { get; set; } = new List<MenuItem>();
}
