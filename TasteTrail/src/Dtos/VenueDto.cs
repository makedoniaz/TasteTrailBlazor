namespace TasteTrailBlazor.Dtos;

public class VenueDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string LogoUrlPath { get; set; } = "https://tastetrailstorage.blob.core.windows.net/venue-logos/default-logo.png";
        public decimal AveragePrice { get; set; }
        public decimal OverallRating { get; set; } = 0;
        public string UserId { get; set; }
    }
