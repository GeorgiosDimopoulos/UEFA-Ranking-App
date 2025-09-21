using API.Data.DTOs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ILogger<TeamsController> _logger;
    private readonly ITeamRepository teamRepository;
    private readonly ICountryRepository countryRepository;

    public TeamsController(ILogger<TeamsController> logger, ITeamRepository teamRepository, ICountryRepository countryRepository)
    {
        _logger = logger;
        this.teamRepository = teamRepository;
        this.countryRepository = countryRepository;
    }

    [HttpGet()]
    public async Task<IEnumerable<TeamResponse>> GetTeams()
    {
        var teams = await teamRepository.GetAllTeams();
        var countries = await countryRepository.GetAllCountries();

        return teams.Select(t => new TeamResponse
        {
            Id = t.Id,
            Name = t.Name,
            IsActive = t.IsActive,
            Points = t.Points,
            Position = t.Position,
            CountryName = countries.FirstOrDefault(c => c.Id == t.CountryId)!.Name,
            Competition = (int)t.Competition
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TeamResponse>> GetTeamById(int id)
    {
        var team = await teamRepository.GetTeamById(id);
        if (team == null)
        {
            _logger.LogWarning("Team with id {Id} not found", id);
            return NotFound();
        }

        var teamCountry = await teamRepository.GetTeamById(id);

        var teamDto = new TeamResponse
        {
            Id = team.Id,
            Name = team.Name,
            IsActive = team.IsActive,
            Points = team.Points,
            Position = team.Position,
            CountryName = teamCountry!.Name,
            Competition = (int)team.Competition
        };

        return Ok(teamDto);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<TeamResponse>> GetTeamById(string n)
    {
        var team = await teamRepository.GetTeamByName(n);
        if (team == null)
        {
            _logger.LogWarning("Team with name: {n} not found", n);
            return NotFound();
        }

        var teamCountry = await teamRepository.GetTeamByName(n);

        var teamDto = new TeamResponse
        {
            Name = team.Name,
            IsActive = team.IsActive,
            Points = team.Points,
            Position = team.Position,
            CountryName = teamCountry!.Name,
            Competition = (int)team.Competition
        };

        return Ok(teamDto);
    }

    [HttpPost(Name = "AddTeam")]
    public async Task<ActionResult<TeamRequest>> AddTeam([FromQuery] TeamRequest t)
    {
        var team = new Team { IsActive = t.IsActive, Name = t.Name, Points = t.Points };
        team.Competition = (Competition)t.Competition;
        var result = await teamRepository.AddTeam(team, t.CountryName);
        if (result == false)
        {
            _logger.LogWarning("Could not add team {Team}", t.Name);
            return BadRequest();
        }
        return Ok();
    }

    [HttpPut("{id:int}", Name = "UpdateTeam")]
    public async Task<ActionResult> UpdateTeam([FromQuery] TeamRequest t, int id)
    {
        var team = new Team { IsActive = t.IsActive, Name = t.Name, Points = t.Points }; // ToDo: add or not Position = t.Position
        var result = await teamRepository.UpdateTeam(team, id);
        if (result == false)
        {
            _logger.LogWarning("Could not update team {Team}", t.Name);
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
            _logger.LogWarning("Could not delete team with id {Id}", id);
            return NotFound();
        }

        return NoContent();
    }
}
