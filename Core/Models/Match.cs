namespace Core.Models;

public class Match
{
    public int Id { get; set; }
    public string HomeTeamName { get; set; } = default!;
    public string AwayTeamName { get; set; } = default!;
    public int Round { get; set; }
    public required Competition Competition { get; set; }   
    public int HomeTeamGoals { get; set; }
    public int AwayTeamGoals { get; set; }
}

public enum MatchResult
{
    HomeWin,
    Draw,
    AwayWin
}