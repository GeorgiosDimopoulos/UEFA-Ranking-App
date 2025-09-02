namespace API.Data.DTOs;

public class TeamDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Points { get; set; }
    public int Position { get; set; }
    public int CountryId { get; set; }
    public int Competition { get; set; }
}
