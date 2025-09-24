using API.Data.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Matches")]
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
    [SwaggerOperation(Tags = new[] { "Matches - Get" })]
    public async Task<List<MatchResponse>> GetAllMatches()
    {
        var matches = await matchRepository.GetAllMatches();

        if (matches is null)
        {
            _logger.LogWarning("No matches found in the database.");
            return [];
        }

        var matchesResponses = matches.Select(m => new MatchResponse
        {
            Id = m.Id,
            HomeTeamName = m.HomeTeamId, // ToDo: set the Name not the Id
            AwayTeamName = m.AwayTeamId,
            Round = m.Round,
            Competition = m.Competition,
            Result = (int)m.Result
        });

        return matchesResponses.ToList();
    }

    [HttpGet("by-country/")]
    [SwaggerOperation(Tags = new[] { "Matches - Get" })]
    public async Task<List<MatchResponse>> GetMatchesByCountry(int id)
    {
        var matches = await matchRepository.GetMatchesByCountryId(id);

        if (matches is null)
        {
            _logger.LogWarning("No matches found in the database.");
            return [];
        }

        var matchesResponses = matches.Select(m => new MatchResponse
        {
            Id = m.Id,
            HomeTeamName = m.HomeTeamId, // ToDo: set the Name not the Id
            AwayTeamName = m.AwayTeamId,
            Competition = m.Competition,
            Round = m.Round,
            Result = (int)m.Result
        });

        return matchesResponses.ToList();
    }

    [HttpGet("by-team")]
    [SwaggerOperation(Tags = new[] { "Matches - Get" })]
    public async Task<List<MatchResponse>> GetMatchesByTeam(int id)
    {
        var matches = await matchRepository.GetMatchesByTeamId(id);

        if (matches is null)
        {
            _logger.LogWarning("No matches found in the database.");
            return [];
        }

        var matchesResponses = matches.Select(m => new MatchResponse
        {
            Id = m.Id,
            HomeTeamName = m.HomeTeamId, // ToDo: set the Name not the Id
            AwayTeamName = m.AwayTeamId,
            Round = m.Round,
            Competition = m.Competition,
            Result = (int)m.Result
        });

        return matchesResponses.ToList();
    }

    [HttpGet("by-round")]
    [SwaggerOperation(Tags = new[] { "Matches - Get" })]
    public async Task<List<MatchResponse>> GetMatchesByRound(int r)
    {
        var matches = await matchRepository.GetMatchesByRound(r);

        if (matches is null)
        {
            _logger.LogWarning("No matches found in the database.");
            return [];
        }

        var matchesResponses = matches.Select(m => new MatchResponse
        {
            Id = m.Id,
            HomeTeamName = m.HomeTeamId, // ToDo: set the Name not the Id
            AwayTeamName = m.AwayTeamId,
            Round = m.Round,
            Competition = m.Competition,
            Result = (int)m.Result
        });

        return matchesResponses.ToList();
    }

    [HttpPost]
    [SwaggerOperation(Tags = new[] { "Matches - Post" })]
    public async Task<ActionResult> AddMatch(MatchRequest matchRequest)
    {
        if (matchRequest is null)
        {
            return BadRequest("Match data is null.");
        }
        var match = new Match
        {
            HomeTeamId = matchRequest.HomeTeamName, // ToDo: set the Name not the Id
            AwayTeamId = matchRequest.AwayTeamName,
            Round = matchRequest.Round,
            Result = (MatchResult)matchRequest.Result,
            Competition = matchRequest.Competition
        };
        var result = await matchRepository.AddMatch(match);
        if (!result)
        {
            _logger.LogError("Failed to add the match to the database.");
            return StatusCode(500, "A problem happened while handling your request.");
        }
        return Ok("Match added successfully.");
    }

    [HttpPut]
    [SwaggerOperation(Tags = new[] { "Matches - Put" })]
    public async Task<ActionResult> UpdateMatch(MatchRequest matchRequest)
    {
        if (matchRequest is null)
        {
            return BadRequest("Match data is null.");
        }
        var match = new Match
        {
            HomeTeamId = matchRequest.HomeTeamName, // ToDo: set the Name not the Id
            AwayTeamId = matchRequest.AwayTeamName,
            Round = matchRequest.Round,
            Result = (MatchResult)matchRequest.Result,
            Competition = matchRequest.Competition
        };
        var result = await matchRepository.AddMatch(match);
        if (!result)
        {
            _logger.LogError("Failed to update the match to the database.");
            return StatusCode(500, "A problem happened while handling your request.");
        }
        return Ok("Match update successfully.");
    }

    [HttpDelete]
    [SwaggerOperation(Tags = new[] { "Matches - Delete" })]
    public async Task<ActionResult> DeleteMatch(int matchId)
    {
        var result = await matchRepository.DeleteMatch(matchId);
        if (!result)
        {
            _logger.LogError("Failed to delete the match in the database.");
            return StatusCode(500, "A problem happened while handling your request.");
        }
        return Ok("Match deleted successfully.");
    }
}
