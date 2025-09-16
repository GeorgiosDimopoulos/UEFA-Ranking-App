namespace API.Data.DTOs;

public class CountryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public int NumberOfActiveTeams { get; set; }
    public int NumberOfInitialTeams { get; set; }
    public double TotalPoints { get; set; }
}
