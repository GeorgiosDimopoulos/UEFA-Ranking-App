namespace Core.Models;
public class Match
{
    public int Id { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public int Round { get; set; }

    // public DateTime Date { get; set; }
    public required Competition Competition { get; set; }
    public required MatchResult Result { get; set; }    
}

public enum MatchResult
{
    HomeWin,
    Draw,
    AwayWin
}

public enum Competition
{
    None = 0,
    ChampionsLeague = 1,
    EuropaLeague = 2,
    ConferenceLeague = 3
}