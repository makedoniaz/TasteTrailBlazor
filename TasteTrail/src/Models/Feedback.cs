using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasteTrailBlazor.Models;

public class Feedback
{
    public int Id { get; set; }

    public string? Text { get; set; }

    public int Rating { get; set; }

    public DateTime CreationDate { get; set; }

    public string? Username { get; set; }

    public string? UserId { get; set; }

    public int VenueId { get; set; }

    public int Likes { get; set; }

    public bool IsLiked { get; set; }
}
