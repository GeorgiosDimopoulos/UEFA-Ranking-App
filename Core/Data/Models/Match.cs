namespace Core.Data.Models
{
    public class Match
    {
        public int Id { get; set; }

        public int ExternalId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }

        public DateTime Date { get; set; }

        public required Competition Competition { get; set; }
        public required MatchResult Result { get; set; }

        public required Team HomeTeam { get; set; }
        public required Team AwayTeam { get; set; }
    }
}
public enum MatchResult
{
    Win = 3,
    Draw = 1,
    Loss = 0
}
public enum Competition
{
    ChampionsLeague = 1,
    EuropaLeague = 2,
    ConferenceLeague = 3
}