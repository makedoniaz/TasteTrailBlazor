using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Dtos;

public class VenueDto
{
    public int CurrentPage { get; set; } 
    public int AmountOfPages { get; set; }  
    public int AmountOfEntities { get; set; }  
    public List<Venue>? Entities { get; set; } 
}
