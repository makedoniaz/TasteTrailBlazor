namespace TasteTrailBlazor.Dtos;

public class UserListDto
{
    public int CurrentPage { get; set; }
    public int AmountOfPages { get; set; }
    public int AmountOfEntities { get; set; }
    public List<UserDto> Entities { get; set; } = new List<UserDto>();
}

