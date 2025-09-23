using API.Data.DTOs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.QueryParameters;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<List<TeamResponse>> GetTeams([FromQuery] TeamQueryParameters queryParameters)
    {
        var teams = await teamRepository.GetAllTeams();
        var countries = await countryRepository.GetAllCountries();

        var teamsPositions = teams.Select(t => t.Points)
                                  .Distinct()
                                  .OrderByDescending(p => p)
                                  .Select((p, i) => new { p, pos = i + 1 })
                                  .ToDictionary(x => x.p, x => x.pos);

        return teams.Select(async t => new TeamResponse
        {
            Id = t.Id,
            Name = t.Name,
            IsActive = t.IsActive,
            Points = t.Points,
            Position = teamsPositions[t.Points],
            CountryName = countries.FirstOrDefault(c => c.Id == t.CountryId)!.Name,
            Competition = (int)t.Competition,
            Matches = queryParameters.IncludeMatches ? await matchRepository.GetMatchesByTeamId(t.Id) : [],
        }).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TeamResponse>> GetTeamById(int id, [FromQuery] TeamQueryParameters queryParameters)
    {
        var team = await teamRepository.GetTeamById(id);
        if (team == null)
        {
            _logger.LogWarning("Team with id {Id} not found", id);
            return NotFound();
        }

        var teamCountry = await countryRepository.GetCountryById(team.CountryId);

        var teamDto = new TeamResponse
        {
            Id = team.Id,
            Name = team.Name,
            IsActive = team.IsActive,
            Points = team.Points,
            Position = await GetTeamPosition(team.Points),
            CountryName = teamCountry!.Name,
            Competition = (int)team.Competition
        };

        if (queryParameters.IncludeMatches)
        {
            teamDto.Matches = team.Matches.ToList();
        }
        
        return Ok(teamDto);
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<TeamResponse>> GetTeamByName(string name, [FromQuery] TeamQueryParameters queryParameters)
    {
        var team = await teamRepository.GetTeamByName(name);
        if (team == null)
        {
            _logger.LogWarning("Team with name: {n} not found", name);
            return NotFound();
        }

        var teamCountry = await countryRepository.GetCountryById(team.CountryId);

        var teamDto = new TeamResponse
        {
            Name = team.Name,
            IsActive = team.IsActive,
            Points = team.Points,
            Position = await GetTeamPosition(team.Points),
            CountryName = teamCountry!.Name, // ToDo: include via JOIN in GetTeamByName
            Competition = (int)team.Competition
        };

        if (queryParameters.IncludeMatches)
        {
            teamDto.Matches = team.Matches.ToList();
        }

        return Ok(teamDto);
    }

    [HttpPost(Name = "AddTeam")]
    public async Task<ActionResult<TeamRequest>> AddTeam([FromQuery] TeamRequest t)
    {
        var team = new Team
        {
            IsActive = t.IsActive,
            Name = t.Name,
            Points = t.Points,
            Competition = (Competition)t.Competition
        };
        var result = await teamRepository.AddTeam(team, t.CountryName);
        if (result == false)
        {
            _logger.LogWarning($"Could not add team {t.Name}");
            return BadRequest();
        }
        return Ok();
    }

    [HttpPut("{id:int}", Name = "UpdateTeam")]
    public async Task<ActionResult> UpdateTeam([FromQuery] TeamRequest t, int id)
    {
        var team = new Team { IsActive = t.IsActive, Name = t.Name, Points = t.Points };
        var result = await teamRepository.UpdateTeam(team, id);
        if (result == false)
        {
            _logger.LogWarning($"Could not update team {t.Name}");
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "DeleteTeam")]
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

    [HttpDelete("by-name/{name}")]
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
