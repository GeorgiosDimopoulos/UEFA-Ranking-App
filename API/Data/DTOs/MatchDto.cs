using Core.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Data.DTOs;

public class MatchResponse
{
    public int Id { get; set; }
    public string HomeTeamName { get; set; } = default!;
    public string AwayTeamName { get; set; } = default!;
    public Round Round { get; set; }

    public string Score { get; set; } = default!;
    public Competition Competition { get; set; }
}

public class MatchRequest
{
    public string HomeTeamName { get; set; } = default!;
    public string AwayTeamName { get; set; } = default!;

    public Round Round { get; set; }

    [SwaggerSchema(Description = "Example format: \"1 - 1\"")]
    public string? Score { get; set; }
    public Competition Competition { get; set; }
}
