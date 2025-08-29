namespace BlazorAppDemo.Models
{
    public class Match
    {
        public int Id { get; set; }
        // public int ExternalId { get; set; }
        public DateTime Date { get; set; }

        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;

        public Team HomeTeam { get; set; } // required 
        public Team AwayTeam { get; set; } // required

        // public Competition Competition { get; set; }

        public MatchResult Result { get; set; }
    }
}
public enum MatchResult
{
    HomeWin,
    AwayWin,
    Draw
}