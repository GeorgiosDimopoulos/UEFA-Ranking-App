using API.Data.DTOs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.QueryParameters;
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
    public async Task<List<TeamResponse>> GetTeams([FromQuery] TeamQueryParameters queryParameters)
    {
        var teams = await teamRepository.GetAllTeams();

        if (teams is null || teams.Count == 0)
        {
            _logger.LogWarning("No teams found in the database.");
            return [];
        }

        var countries = await countryRepository.GetAllCountries();

        Dictionary<int, List<Match>> matchesByTeams = [];
        if (queryParameters.IncludeMatches)
        {
            foreach (var t in teams)
            {
                var matchByTeam = await matchRepository.GetMatchesByTeamName(t.Name);
                matchesByTeams[t.Id] = matchByTeam;
            }
        }

        if (queryParameters.Competition != null)
        {
            teams = (teams.Where(t => t.Competition == queryParameters.Competition)).ToList();
        }

        var teamsPositions = teams.OrderByDescending(t => t.Points)
                                  .ThenByDescending(t => matchesByTeams[t.Id].Sum(m => m.HomeTeamName == t.Name
                                                    ? (m.HomeTeamGoals ?? 0) - (m.AwayTeamGoals ?? 0) : (m.AwayTeamGoals ?? 0) - (m.HomeTeamGoals ?? 0)))
                                  .Select((t, i) => new { t.Id, Position = i + 1 })
                                  .ToDictionary(x => x.Id, x => x.Position);

        return teams.Select(t => new TeamResponse
        {
            Id = t.Id,
            Name = t.Name,
            IsActive = t.IsActive,
            CountryPoints = countries.FirstOrDefault(c => c.Id == t.CountryId)!.TotalPoints,
            Points = t.Points,
            GoalsDifference = matchesByTeams[t.Id].Sum(m => m.HomeTeamName == t.Name ? (m.HomeTeamGoals ?? 0) - (m.AwayTeamGoals ?? 0) : (m.AwayTeamGoals ?? 0) - (m.HomeTeamGoals ?? 0)),
            Position = teamsPositions[t.Id],
            CountryName = countries.FirstOrDefault(c => c.Id == t.CountryId)!.Name,
            Competition = t.Competition,
            Matches = (queryParameters.IncludeMatches && t.IsActive) ? matchesByTeams[t.Id] : null,
            MatchesPlayed = (t.IsActive && queryParameters.IncludeMatches) ? matchesByTeams[t.Id].Count(m => m.HomeTeamGoals != null && m.AwayTeamGoals != null) : 0
        }).ToList();
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Tags = new[] { "Teams - Get" })]
    public async Task<ActionResult<TeamResponse>> GetTeamById(int id, [FromQuery] TeamQueryParameters queryParameters)
    {
        var team = await teamRepository.GetTeamById(id);
        if (team == null)
        {
            _logger.LogWarning($"Team with id {id} not found", id);
            return NotFound();
        }

        var teamCountry = await countryRepository.GetCountryById(team.CountryId);

        Dictionary<int, List<Match>> teamMatches = [];
        if (queryParameters.IncludeMatches)
        {
            var matchByTeam = await matchRepository.GetMatchesByTeamName(team.Name);
            teamMatches[team.Id] = matchByTeam;
        }

        var teamDto = new TeamResponse
        {
            Id = team.Id,
            Name = team.Name,
            IsActive = team.IsActive,
            Points = team.Points,
            Position = await GetTeamPosition(team.Points),
            CountryName = teamCountry!.Name,
            Competition = team.Competition,
            Matches = (queryParameters.IncludeMatches && team.IsActive) ? teamMatches[team.Id] : null,
            MatchesPlayed = (team.IsActive && queryParameters.IncludeMatches) ? teamMatches[team.Id].Count(m => m.HomeTeamGoals != null && m.AwayTeamGoals != null) : 0
        };

        return Ok(teamDto);
    }

    [HttpGet("by-country/{countryId:int}")]
    [SwaggerOperation(Tags = new[] { "Teams - Get" })]
    public async Task<ActionResult<List<TeamResponse>>> GetTeamsByCountryId(int countryId, [FromQuery] TeamQueryParameters queryParameters)
    {
        var teamCountry = await countryRepository.GetCountryById(countryId);
        if (teamCountry == null)
        {
            _logger.LogWarning($"Country with id: {countryId} not found");
            return NotFound();
        }

        var teams = await teamRepository.GetTeamsByCountryId(countryId);
        if (teams == null)
        {
            _logger.LogWarning($"Teams with country id: {countryId} not found");
            return NotFound();
        }

        Dictionary<int, List<Match>> matchesByTeams = [];
        if (queryParameters.IncludeMatches)
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
                Name = team.Name,
                IsActive = team.IsActive,
                Points = team.Points,
                Position = await GetTeamPosition(team.Points),
                CountryName = teamCountry.Name,
                Competition = team.Competition,
                Matches = (queryParameters.IncludeMatches && team.IsActive) ? matchesByTeams[team.Id] : null,
                MatchesPlayed = (team.IsActive && queryParameters.IncludeMatches) ? matchesByTeams[team.Id].Count(m => m.HomeTeamGoals != null && m.AwayTeamGoals != null) : 0
            };

            teamsDto.Add(teamDto);
        }

        return Ok(teamsDto);
    }

    [HttpGet("{name}")]
    [SwaggerOperation(Tags = new[] { "Teams - Get" })]
    public async Task<ActionResult<TeamResponse>> GetTeamByName(string name, [FromQuery] TeamQueryParameters queryParameters)
    {
        var team = await teamRepository.GetTeamByName(name);
        if (team == null)
        {
            _logger.LogWarning("Team with name: {n} not found", name);
            return NotFound();
        }

        var teamCountry = await countryRepository.GetCountryById(team.CountryId);

        Dictionary<int, List<Match>> teamMatches = [];
        if (queryParameters.IncludeMatches)
        {
            var matchByTeam = await matchRepository.GetMatchesByTeamName(team.Name);
            teamMatches[team.Id] = matchByTeam;
        }

        var teamDto = new TeamResponse
        {
            Name = team.Name,
            IsActive = team.IsActive,
            Points = team.Points,
            Position = await GetTeamPosition(team.Points),
            CountryName = teamCountry!.Name, // ToDo: include via JOIN in GetTeamByName
            Competition = team.Competition,
            CountryPoints = teamCountry!.TotalPoints,
            Matches = (queryParameters.IncludeMatches && team.IsActive) ? teamMatches[team.Id] : null,
            MatchesPlayed = (team.IsActive && queryParameters.IncludeMatches) ? teamMatches[team.Id].Count(m => m.HomeTeamGoals != null && m.AwayTeamGoals != null) : 0
        };

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
            Points = t.Points,
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
        var team = new Team { Competition = t.Competition, IsActive = t.IsActive, Name = t.Name, Points = t.Points };
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

    private async Task<int> GetTeamPosition(int points)
    {
        var teamsPoints = await teamRepository.GetTeamsNamesAndPoints();
        return 1 + teamsPoints.Values.Count(p => p > points);
    }
}
