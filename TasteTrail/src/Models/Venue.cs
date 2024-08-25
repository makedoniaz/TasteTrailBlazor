using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasteTrailBlazor.Models;

public class Venue
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string? ContactNumber { get; set; }

    public string? Email { get; set; }

    public string? LogoUrlPath { get; set; }

    public float AveragePrice { get; set; }

    public float OverallRating { get; set; }

    public ICollection<Menu>? Menus { get; set; }

    public ICollection<VenuePhoto>? Photos { get; set; }

    public ICollection<Feedback>? Feedbacks { get; set; }

    public string? UserId { get; set; }
}

public class VenuePhoto
{
    public int Id { get; set; }

    public int VenueId { get; set; }

    public string? PhotoUrlPath { get; set; }
}

public class VenueCreateDto
{
    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }
    
    public string? Email { get; set; }

    public string? ContactNumber { get; set; }

    public float AveragePrice { get; set; }
}
