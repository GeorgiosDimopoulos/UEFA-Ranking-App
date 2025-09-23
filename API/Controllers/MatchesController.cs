using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly ILogger<MatchesController> _logger;
    private readonly IMatchRepository matchRepository;

    public MatchesController(IMatchRepository matchRepository, ILogger<MatchesController> logger)
    {
        this.matchRepository = matchRepository;
        _logger = logger;
    }

    [HttpGet()]
    public async Task<List<Match>> GetAllMatches()
    {
        var matches = await matchRepository.GetAllMatches();
        return matches;
    }

    [HttpGet("by-country/")]
    public async Task<List<Match>> GetMatchesByCountry(int id)
    {
        var matches = await matchRepository.GetMatchesByCountryId(id);
        return matches;
    }

    [HttpGet("by-team")]
    public async Task<List<Match>> GetMatchesByTeam(int id)
    {
        var matches = await matchRepository.GetMatchesByTeamId(id);
        return matches;
    }

    [HttpGet("by-round")]
    public async Task<List<Match>> GetMatchesByRound(int r)
    {
        var matches = await matchRepository.GetMatchesByRound(r);
        return matches;
    }
}
