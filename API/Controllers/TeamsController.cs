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

    [HttpGet(Name = "Teams")]
    public async Task<IEnumerable<Team>> GetTeams()
    {
        return await teamRepository.GetAllTeams();
    }

    [HttpGet("{id :int}", Name = "TeamById")]
    public async Task<ActionResult<Team>> GetTeamById(int id)
    {
        var team = await teamRepository.GetTeamById(id);
        if (team == null)
        {
            _logger.LogWarning("Team with id {Id} not found", id);
            return NotFound();
        }

        return Ok(team);
    }

    [HttpPost(Name = "AddTeam")]
    public async Task<ActionResult<Team>> AddTeam([FromBody] Team t, string countryName, Competition c, int pos)
    {
        var result = await teamRepository.AddTeam(t, countryName, c, pos);
        if (result == false)
        {
            _logger.LogWarning("Could not add team {Team}", t.Name);
            return BadRequest();
        }
        return Ok();
    }

    [HttpPut("{id:int}", Name = "UpdateTeam")]
    public async Task<ActionResult> UpdateTeam(Team t, int id)
    {
        t.Id = id;
        var result = await teamRepository.UpdateTeam(t);
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
