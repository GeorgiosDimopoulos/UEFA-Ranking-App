using API.Data.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ILogger<TeamsController> _logger;
    private readonly ITeamRepository teamRepository;

    public TeamsController(ILogger<TeamsController> logger, ITeamRepository teamRepository)
    {
        _logger = logger;
        this.teamRepository = teamRepository;
    }

    [HttpGet()]
    public async Task<IEnumerable<TeamDto>> GetTeams()
    {
        var teams = await teamRepository.GetAllTeams();
        return teams.Select(t => new TeamDto
        {
            Name = t.Name,
            IsActive = t.IsActive,
            Points = t.Points,
            Position = t.Position,
            CountryName = t.Country.Name,
            Competition = (int)t.Competition
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TeamDto>> GetTeamById(int id)
    {
        var team = await teamRepository.GetTeamById(id);
        if (team == null)
        {
            _logger.LogWarning("Team with id {Id} not found", id);
            return NotFound();
        }

        var teamDto = new TeamDto
        {
            Name = team.Name,
            IsActive = team.IsActive,
            Points = team.Points,
            Position = team.Position,
            CountryName = team.Country.Name,
            Competition = (int)team.Competition
        };

        return Ok(teamDto);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<TeamDto>> GetTeamById(string n)
    {
        var team = await teamRepository.GetTeamByName(n);
        if (team == null)
        {
            _logger.LogWarning("Team with name: {n} not found", n);
            return NotFound();
        }

        var teamDto = new TeamDto
        {
            Name = team.Name,
            IsActive = team.IsActive,
            Points = team.Points,
            Position = team.Position,
            CountryName = team.Country.Name,
            Competition = (int)team.Competition
        };

        return Ok(teamDto);
    }

    [HttpPost(Name = "AddTeam")]
    public async Task<ActionResult<TeamDto>> AddTeam([FromQuery] TeamDto t)
    {
        var team = new Team { IsActive = t.IsActive, Name = t.Name, Points = t.Points, Position = t.Position };
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
    public async Task<ActionResult> UpdateTeam([FromQuery] TeamDto t, int id)
    {
        var team = new Team { IsActive = t.IsActive, Name = t.Name, Points = t.Points, Position = t.Position };
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
