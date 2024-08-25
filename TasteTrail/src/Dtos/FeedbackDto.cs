using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Dtos;

public class FeedbackDto
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public int Rating { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }
    }
