namespace API.Data.DTOs;

public class CountryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public int NumberOfActiveTeams { get; set; }
    public int NumberOfInitialTeams { get; set; }
    public int TotalPoints { get; set; }
}

public class CountryRequest
{
    public string Name { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
}
