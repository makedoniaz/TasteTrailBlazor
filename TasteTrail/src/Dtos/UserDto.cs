using TasteTrailBlazor.Models;

namespace TasteTrailBlazor.Dtos;


public class UserDto
{
    public  User? User { get; set; }
    public  ICollection<string>? Roles { get; set; }
}

