namespace TasteTrailBlazor.Models;

public class AccessToken
{ 
    public Guid Refresh { get; set; }
    public string Jwt { get; set; }
}
