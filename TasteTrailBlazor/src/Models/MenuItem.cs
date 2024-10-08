using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasteTrailBlazor.Models;

public class MenuItem
{

    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public string? ImageUrlPath { get; set; }

    public float Price { get; set; }

    public int Likes { get; set; }

    public required int MenuId { get; set; }

    public required string UserId { get; set; }

    public bool IsLiked { get; set; }
}