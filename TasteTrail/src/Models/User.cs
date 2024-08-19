using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasteTrailBlazor.Models;

public class User  
{
    public bool IsBanned { get; set; }

    public bool IsMuted { get; set; }

    public string? AvatarPath { get; set; }

    public ICollection<Feedback> Feedbacks { get; set; }

    public ICollection<Venue> Venues { get; set; }

    public ICollection<AccessToken> RefreshTokens { get; set; }

    public ICollection<MenuItem> MenuItems { get; set; }

    public ICollection<Menu> Menus { get; set; }
}