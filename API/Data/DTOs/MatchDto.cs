using Core.Models;

namespace API.Data.DTOs;

public class MatchResponse
{
    public int Id { get; set; }
    public int HomeTeamName { get; set; }
    public int AwayTeamName { get; set; }
    public int Round { get; set; }
    public int Result { get; set; }
    public Competition Competition { get; set; }
}

public class MatchRequest
{
    public int HomeTeamName { get; set; }
    public int AwayTeamName { get; set; }
    public int Round { get; set; }
    public int Result { get; set; }
    public Competition Competition { get; set; }
}
