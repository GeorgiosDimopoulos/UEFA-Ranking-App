using Core.Models;

namespace API.Data.DTOs;

public class MatchResponse
{
    public int Id { get; set; }
    public string HomeTeamName { get; set; } = default!;
    public string AwayTeamName { get; set; } = default!;
    public int Round { get; set; }
    public int Result { get; set; }
    public Competition Competition { get; set; }
}

public class MatchRequest
{
    public string HomeTeamName { get; set; } = default!;
    public string AwayTeamName { get; set; } = default!;
    public int Round { get; set; }
    public int Result { get; set; }
    public Competition Competition { get; set; }
}
