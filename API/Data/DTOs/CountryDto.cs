namespace API.Data.DTOs;

public class CountryDto
{
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public int NumberOfTeams { get; set; }
    public double TotalPoints { get; set; }
}
