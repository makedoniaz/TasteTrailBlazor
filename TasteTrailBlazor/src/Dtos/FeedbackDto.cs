using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Dtos;


    public class FeedbackDto
{
    public int CurrentPage { get; set; }
    public int AmountOfPages { get; set; }
    public int AmountOfEntities { get; set; }
    public List<Feedback> Entities { get; set; } = new List<Feedback>();
}

public class FeedbackCreateDto
{
    public string? Text { get; set; }

    public int Rating { get; set; }
    
    public int VenueId { get; set; }
}
