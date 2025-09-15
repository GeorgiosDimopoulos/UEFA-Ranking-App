namespace API.Data.DTOs;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Points { get; set; }
    public int Position { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public int Competition { get; set; }

    //public List<MatchDto> Matches { get; set; } = [];
}
