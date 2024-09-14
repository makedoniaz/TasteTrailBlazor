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
    public double Longtitude { get; set; }

    public double Latitude { get; set; }

    public string? LogoUrlPath { get; set; }
    public float AveragePrice { get; set; }

    public float Rating { get; set; }

    public DateTime? CreationDate { get; set; }
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
    public double Longtitude { get; set; }

    public double Latitude { get; set; }
}
