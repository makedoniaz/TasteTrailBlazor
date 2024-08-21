using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasteTrailBlazor.Models;

public class Menu
{
    public int Id { get; set; }

    public   string Name { get; set; }

    public string? Description { get; set; }

    public int VenueId { get; set; }

    public string UserId { get; set; } 
    public ICollection<MenuItem> MenuItems { get; set; }
}