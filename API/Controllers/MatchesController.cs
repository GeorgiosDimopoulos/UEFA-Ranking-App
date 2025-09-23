using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MatchesController : ControllerBase
{
    private readonly IMatchRepository matchRepository;

    public MatchesController(IMatchRepository matchRepository)
    {
        this.matchRepository = matchRepository;
    }

    [HttpGet()]
    public async Task<List<Match>> GetAllMatches()
    {
        var matches = await matchRepository.GetAllMatches();
        return matches;
    }
}
