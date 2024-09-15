namespace TasteTrailBlazor.Dtos;

public class MenuUpdateDto
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int VenueId { get; set; }

    public string? UserId { get; set; }

}