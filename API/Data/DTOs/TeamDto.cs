using Core.Models;

namespace API.Data.DTOs;

public class TeamResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Points { get; set; }
    public int Position { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public int CountryPoints { get; set; }
    public int MatchesPlayed { get; set; } // => Matches?.Count() ?? 0;

    public Competition Competition { get; set; }    
    
    public List<Match>? Matches { get; set; }
}

public class TeamRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Points { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public Competition Competition { get; set; }
}
