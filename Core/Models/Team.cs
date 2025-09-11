namespace Core.Models;

public class Team
{
    public int Id { get; set; }
    public int ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Points { get; set; }
    public int Position { get; set; }

    public int CountryId { get; set; }
    public Country Country { get; set; } = new();

    public Competition Competition { get; set; }
    public IEnumerable<Match> Matches { get; set; } = [];
}
