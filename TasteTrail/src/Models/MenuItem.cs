using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasteTrailBlazor.Models;

public class MenuItem
{
    public int Id { get; set; }

    public   string Name { get; set; }

    public string? Description { get; set; }

    public float Price { get; set; }

    public int PopularityRate { get; set; }

    public int MenuId { get; set; }

    public string UserId { get; set; }
}