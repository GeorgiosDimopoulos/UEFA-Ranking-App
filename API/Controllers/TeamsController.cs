using API.Data.DTOs;
using Core.Interfaces;
using Core.Models;
using Core.QueryParameters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Teams")]
public class TeamsController : ControllerBase
{
    private readonly ILogger<TeamsController> _logger;
    private readonly ITeamRepository teamRepository;
    private readonly ICountryRepository countryRepository;
    private readonly IMatchRepository matchRepository;

    public TeamsController(ILogger<TeamsController> logger, ITeamRepository teamRepository, ICountryRepository countryRepository, IMatchRepository matchRepository)
    {
        _logger = logger;
        this.teamRepository = teamRepository;
        this.countryRepository = countryRepository;
        this.matchRepository = matchRepository;
    }

    [HttpGet()]
    [SwaggerOperation(Tags = new[] { "Teams - Get" })]
    public async Task<List<TeamResponse>> GetTeams([FromQuery] TeamQueryParameters tqueryParameters)
    {
        var teams = await teamRepository.GetAllTeams(tqueryParameters);

        if (teams is null || teams.Count == 0)
        {
            _logger.LogWarning("No teams found in the database.");
            return [];
        }

        var countries = await countryRepository.GetAllCountries(new() { IncludeTeams = false, IncludeMatches = false });

        Dictionary<int, List<Match>> matchesByTeams = [];
        foreach (var t in teams)
        {
            var matchByTeam = await matchRepository.GetMatchesByTeamName(t.Name);
            matchesByTeams[t.Id] = matchByTeam;
        }

        if (tqueryParameters.Competition != null)
        {
            teams = (teams.Where(t => t.Competition == tqueryParameters.Competition)).ToList();
        }

        var competitionTeams = teams.GroupBy(t => t.Competition);

        Dictionary<int, int> pointsByTeamId = [];
        foreach (var t in teams)
        {
            var teamPoints = GetTeamPoints(matchesByTeams[t.Id], t.Name);
            pointsByTeamId[t.Id] = teamPoints;
        }

        var teamsPositions = competitionTeams.SelectMany(gr => gr
                                             .OrderByDescending(t => pointsByTeamId[t.Id])
                                             .ThenByDescending(t => CalculateGoalsDifference(t, matchesByTeams[t.Id]))
                                             .Select((t, i) => new { t.Id, Position = i + 1 }))
                                             .ToDictionary(x => x.Id, x => x.Position);

        return teams.Select(t => new TeamResponse
        {
            Id = t.Id,
            Name = t.Name,
            IsActive = t.IsActive,
            Points = pointsByTeamId[t.Id],
            CountryPoints = countries.FirstOrDefault(c => c.Id == t.CountryId)!.TotalPoints,
            GoalsDifference = CalculateGoalsDifference(t, matchesByTeams[t.Id]),
            Position = teamsPositions[t.Id],
            CountryName = countries.FirstOrDefault(c => c.Id == t.CountryId)!.Name,
            Competition = t.Competition,
            Matches = (tqueryParameters.IncludeMatches && t.IsActive) ? matchesByTeams[t.Id] : null,
            MatchesPlayed = (t.IsActive && tqueryParameters.IncludeMatches) ? matchesByTeams[t.Id].Count(m => m.HomeTeamGoals != null && m.AwayTeamGoals != null) : 0
        }).ToList();
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Tags = new[] { "Teams - Get" })]
    public async Task<ActionResult<TeamResponse>> GetTeamById(int id, [FromQuery] TeamQueryParameters tqueryParameters)
    {
        var team = await teamRepository.GetTeamById(id, tqueryParameters);
        if (team == null)
        {
            _logger.LogWarning($"Team with id {id} not found", id);
            return NotFound();
        }

        var teamCountry = await countryRepository.GetCountryById(team.CountryId, new() { IncludeMatches = false, IncludeTeams = false });

        Dictionary<int, List<Match>> teamMatches = [];
        if (tqueryParameters.IncludeMatches)
        {
            var matchByTeam = await matchRepository.GetMatchesByTeamName(team.Name);
            teamMatches[team.Id] = matchByTeam;
        }

        var teamDto = new TeamResponse
        {
            Id = team.Id,
            Name = team.Name,
            IsActive = team.IsActive,
            CountryName = teamCountry!.Name,
            Competition = team.Competition,
            Matches = (tqueryParameters.IncludeMatches && team.IsActive) ? teamMatches[team.Id] : null,
            MatchesPlayed = (team.IsActive && tqueryParameters.IncludeMatches) ? teamMatches[team.Id].Count(m => m.HomeTeamGoals != null && m.AwayTeamGoals != null) : 0
        };

        teamDto.Points = GetTeamPoints(team.Matches, team.Name);
        return Ok(teamDto);
    }

    [HttpGet("by-country/{countryId:int}")]
    [SwaggerOperation(Tags = new[] { "Teams - Get" })]
    public async Task<ActionResult<List<TeamResponse>>> GetTeamsByCountryId(int countryId, [FromQuery] TeamQueryParameters tqueryParameters)
    {
        var teamCountry = await countryRepository.GetCountryById(countryId, new() { IncludeTeams = false, IncludeMatches = false });
        if (teamCountry == null)
        {
            _logger.LogWarning($"Country with id: {countryId} not found");
            return NotFound();
        }

        var teams = await teamRepository.GetTeamsByCountryId(countryId, tqueryParameters);
        if (teams == null)
        {
            _logger.LogWarning($"Teams with country id: {countryId} not found");
            return NotFound();
        }

        Dictionary<int, List<Match>> matchesByTeams = [];
        if (tqueryParameters.IncludeMatches)
        {
            foreach (var t in teams)
            {
                var matchByTeam = await matchRepository.GetMatchesByTeamName(t.Name);
                matchesByTeams[t.Id] = matchByTeam;
            }
        }

        var teamsDto = new List<TeamResponse>();
        foreach (var team in teams!)
        {
            var teamDto = new TeamResponse
            {
                Id = team!.Id,
                Points = GetTeamPoints(team.Matches, team.Name),
                Name = team.Name,
                IsActive = team.IsActive,
                CountryName = teamCountry.Name,
                Competition = team.Competition,
                Matches = (tqueryParameters.IncludeMatches && team.IsActive) ? matchesByTeams[team.Id] : null,
                MatchesPlayed = (team.IsActive && tqueryParameters.IncludeMatches) ? matchesByTeams[team.Id].Count(m => m.HomeTeamGoals != null && m.AwayTeamGoals != null) : 0
            };

            teamDto.Position = await GetTeamPosition(teamDto.Name, teamDto.Competition, tqueryParameters);
            teamsDto.Add(teamDto);
        }

        return Ok(teamsDto);
    }

    [HttpGet("{name}")]
    [SwaggerOperation(Tags = new[] { "Teams - Get" })]
    public async Task<ActionResult<TeamResponse>> GetTeamByName(string name, [FromQuery] TeamQueryParameters tqueryParameters)
    {
        var team = await teamRepository.GetTeamByName(name, tqueryParameters);
        if (team == null)
        {
            _logger.LogWarning("Team with name: {n} not found", name);
            return NotFound();
        }

        var teamCountry = await countryRepository.GetCountryById(team.CountryId, new() { IncludeMatches = false, IncludeTeams = false });

        if (teamCountry is null)
        {
            _logger.LogWarning($"Team country not found {name}");
            return NotFound();
        }

        Dictionary<int, List<Match>> teamMatches = [];
        if (tqueryParameters.IncludeMatches)
        {
            var matchByTeam = await matchRepository.GetMatchesByTeamName(team.Name);
            teamMatches[team.Id] = matchByTeam;
        }

        var teamDto = new TeamResponse
        {
            Name = team.Name,
            IsActive = team.IsActive,
            Points = GetTeamPoints(team.Matches, team.Name),
            CountryName = teamCountry.Name, // ToDo: include via JOIN in GetTeamByName
            Competition = team.Competition,
            CountryPoints = teamCountry.TotalPoints,
            Matches = (tqueryParameters.IncludeMatches && team.IsActive) ? teamMatches[team.Id] : null,
            MatchesPlayed = (team.IsActive && tqueryParameters.IncludeMatches) ? teamMatches[team.Id].Count(m => m.HomeTeamGoals != null && m.AwayTeamGoals != null) : 0
        };

        teamDto.Position = await GetTeamPosition(teamDto.Name, team.Competition, tqueryParameters);
        return Ok(teamDto);
    }

    [HttpPost(Name = "AddTeam")]
    [SwaggerOperation(Tags = new[] { "Teams - Post" })]
    public async Task<ActionResult<TeamRequest>> AddTeam([FromQuery] TeamRequest t)
    {
        if (!Enum.GetValues<Competition>().Contains(t.Competition))
        {
            t.Competition = 0;
        }
        var team = new Team
        {
            IsActive = t.IsActive,
            Name = t.Name,
            Competition = t.Competition
        };
        var result = await teamRepository.AddTeam(team, t.CountryName);
        if (result == false)
        {
            _logger.LogWarning($"Could not add team {t.Name}");
            return BadRequest();
        }
        return Ok();
    }

    [HttpPut("{name}")]
    [SwaggerOperation(Tags = new[] { "Teams - Put" })]
    public async Task<ActionResult> UpdateTeam(string name, [FromQuery] TeamRequest t)
    {
        var team = new Team { Competition = t.Competition, IsActive = t.IsActive, Name = t.Name };
        var result = await teamRepository.UpdateTeam(team, name);
        if (result == false)
        {
            _logger.LogWarning($"Could not update team {t.Name}");
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Tags = new[] { "Teams - Delete" })]
    public async Task<ActionResult> DeleteTeam(int id)
    {
        var result = await teamRepository.DeleteTeam(id);
        if (result == false)
        {
            _logger.LogWarning($"Could not delete team with id {id}");
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{name}")]
    [SwaggerOperation(Tags = new[] { "Teams - Delete" })]
    public async Task<ActionResult> DeleteTeamByName(string n)
    {
        var result = await teamRepository.DeleteTeamByName(n);
        if (result == false)
        {
            _logger.LogWarning($"Could not delete country with name {n}");
            return NotFound();
        }

        return NoContent();
    }

    private static int CalculateGoalsDifference(Team t, List<Match> matches)
    {
        return matches.Sum(m => m.HomeTeamName.Equals(t.Name) ? (m.HomeTeamGoals ?? 0) - (m.AwayTeamGoals ?? 0) : (m.AwayTeamGoals ?? 0) - (m.HomeTeamGoals ?? 0));
    }

    private async Task<int> GetTeamPosition(string teamName, Competition competition, TeamQueryParameters queryParameters)
    {
        var teams = await teamRepository.GetAllTeams(queryParameters);
        var competitionTeams = teams.Where(t => t.Competition == competition);
        var teamNameAndPointsDict = new Dictionary<string, int>();
        var compMatches = await matchRepository.GetMatchesByCompetition(competition);
        compMatches = compMatches.Where(m => m.HomeTeamGoals != null && m.AwayTeamGoals != null).ToList();
        foreach (var team in competitionTeams)
        {
            var teamMatches = compMatches.Where(m => m.HomeTeamName.Equals(team.Name, StringComparison.OrdinalIgnoreCase) || m.AwayTeamName.Equals(team.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            var teamPoints = GetTeamPoints(teamMatches, team.Name);
            teamNameAndPointsDict[team.Name] = teamPoints;
        }

        var teamPosition = teamNameAndPointsDict.OrderByDescending(kv => kv.Value)
                                                .ThenBy(kv => kv.Key)
                                                .Select((kv, index) => new { TeamName = kv.Key, Position = index + 1 })
                                                .FirstOrDefault(kv => kv.TeamName.Equals(teamName))?.Position ?? 0;
        return teamPosition;
    }

    private int GetTeamPoints(IEnumerable<Match> matches, string teamName)
    {
        int points = 0;
        foreach (var match in matches)
        {
            if (match.HomeTeamGoals == null || match.AwayTeamGoals == null)
                continue;

            if (match.HomeTeamName.Equals(teamName))
            {
                if (match.HomeTeamGoals > match.AwayTeamGoals)
                {
                    points += 3;
                }
                else if (match.HomeTeamGoals == match.AwayTeamGoals)
                {
                    points += 1;
                }
            }
            else if (match.AwayTeamName.Equals(teamName))
            {
                if (match.HomeTeamGoals < match.AwayTeamGoals)
                {
                    points += 3;
                }
                else if (match.HomeTeamGoals == match.AwayTeamGoals)
                {
                    points += 1;
                }
            }
        }
        return points;
    }
}
