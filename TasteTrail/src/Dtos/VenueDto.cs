namespace TasteTrailBlazor.Dtos;

public class VenueDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string LogoUrlPath { get; set; } =
        "https://tastetrailstorage.blob.core.windows.net/venue-logos/default-logo.png";
    public decimal AveragePrice { get; set; }
    public decimal OverallRating { get; set; } = 0;

    public string UserId { get; set; } = string.Empty;
}
