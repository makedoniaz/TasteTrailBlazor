using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Dtos;

public class MenuDto
{
    public int CurrentPage { get; set; }
    public int AmountOfPages { get; set; }
    public int AmountOfEntities { get; set; }
    public List<Menu> Entities { get; set; } = new List<Menu>();
}
