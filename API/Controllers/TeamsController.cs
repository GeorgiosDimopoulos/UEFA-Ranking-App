using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
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

    [HttpGet(Name = "TeamById")]
    public async Task<Team> GetTeamById(int id)
    {
        return await teamRepository.GetTeamById(id) ?? new();
    }

    [HttpPost(Name = "AddTeam")]
    public async Task GetTeamByName(Team t, string countryName, Competition c, int pos)
    {
        await teamRepository.AddTeam(t, countryName, c, pos);
    }

    [HttpPut(Name = "UpdateTeam")]
    public async Task UpdateTeam(Team t)
    {
        await teamRepository.UpdateTeam(t);
    }

    [HttpDelete(Name = "DeleteTeam")]
    public async Task DeleteTeam(int id)
    {
        await teamRepository.DeleteTeam(id);
    }
}
