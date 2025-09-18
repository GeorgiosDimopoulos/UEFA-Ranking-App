namespace Core.Models;

public class Country
{
    public string Name { get; set; } = string.Empty;
    public int Id { get; set; }
    public int Position { get; set; }
    public int TotalPoints { get; set; }
    
    public IEnumerable<Team> Teams { get; set; } = [];
}
