namespace TasteTrailBlazor.Dtos;

public class MenuDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int VenueId { get; set; }
}