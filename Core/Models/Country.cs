namespace Core.Models;

public class Country
{
    public string Name { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public int ExternalId { get; set; }
    public int Position { get; set; }
    public IEnumerable<Team> Teams { get; set; } = [];
    public int NumberOfTeams => Teams.Count();
    public int TotalPoints => Teams.Sum(t => t.Points);
}
