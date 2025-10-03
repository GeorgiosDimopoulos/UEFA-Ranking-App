using API.Data.DTOs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.QueryParameters;
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
    private readonly ITeamRepository teamRepository;

    public MatchesController(IMatchRepository matchRepository, ILogger<MatchesController> logger, ITeamRepository teamRepository)
    {
        this.matchRepository = matchRepository;
        this.teamRepository = teamRepository;
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
            HomeTeamName = m.HomeTeamName,
            AwayTeamName = m.AwayTeamName,
            Round = (Round)m.Round,
            Competition = m.Competition,
            Score = $"{m.HomeTeamGoals}-{m.AwayTeamGoals}"
        });

        return matchesResponses.ToList();
    }

    [HttpGet("by-country/")]
    [SwaggerOperation(Tags = new[] { "Matches - Get" })]
    public async Task<List<MatchResponse>> GetMatchesByCountry(int countryId)
    {
        var matches = await matchRepository.GetMatchesByCountryId(countryId);

        if (matches is null)
        {
            _logger.LogWarning($"No matches found for country with id: {countryId} in the database.");
            return [];
        }

        var matchesResponses = matches.Select(m => new MatchResponse
        {
            Id = m.Id,
            HomeTeamName = m.HomeTeamName,
            AwayTeamName = m.AwayTeamName,
            Competition = m.Competition,
            Round = (Round)m.Round,
            Score = $"{m.HomeTeamGoals}-{m.AwayTeamGoals}"
        });

        return matchesResponses.ToList();
    }

    [HttpGet("by-team")]
    [SwaggerOperation(Tags = new[] { "Matches - Get" })]
    public async Task<List<MatchResponse>> GetMatchesByTeam(string team)
    {
        var matches = await matchRepository.GetMatchesByTeamName(team);

        if (matches is null)
        {
            _logger.LogWarning($"No matches found for {team} team in the database.");
            return [];
        }

        var matchesResponses = matches.Select(m => new MatchResponse
        {
            Id = m.Id,
            HomeTeamName = m.HomeTeamName,
            AwayTeamName = m.AwayTeamName,
            Round = (Round)m.Round,
            Competition = m.Competition,
            Score = $"{m.HomeTeamGoals}-{m.AwayTeamGoals}"
        });

        return matchesResponses.ToList();
    }

    [HttpGet("by-round")]
    [SwaggerOperation(Tags = new[] { "Matches - Get" })]
    public async Task<List<MatchResponse>> GetMatchesByRound(int round)
    {
        var matches = await matchRepository.GetMatchesByRound(round);

        if (matches is null)
        {
            _logger.LogWarning($"No matches found for {round} in the database.");
            return [];
        }

        var matchesResponses = matches.Select(m => new MatchResponse
        {
            Id = m.Id,
            HomeTeamName = m.HomeTeamName,
            AwayTeamName = m.AwayTeamName,
            Round = (Round)m.Round,
            Competition = m.Competition,
            Score = $"{m.HomeTeamGoals}-{m.AwayTeamGoals}"
        });

        return matchesResponses.ToList();
    }

    [HttpGet("by-competition")]
    [SwaggerOperation(Tags = new[] { "Matches - Get" })]
    public async Task<List<MatchResponse>> GetMatchesByCompetition(Competition competition)
    {
        var matches = await matchRepository.GetMatchesByCompetition(competition);

        if (matches is null)
        {
            _logger.LogWarning($"No matches found for that {competition} in the database.");
            return [];
        }

        var matchesResponses = matches.Select(m => new MatchResponse
        {
            Id = m.Id,
            HomeTeamName = m.HomeTeamName,
            AwayTeamName = m.AwayTeamName,
            Round = (Round)m.Round,
            Competition = m.Competition,
            Score = $"{m.HomeTeamGoals}-{m.AwayTeamGoals}"
        });

        return matchesResponses.ToList();
    }

    [HttpPost]
    [SwaggerOperation(Tags = new[] { "Matches - Post" })]
    public async Task<ActionResult> AddMatch([FromQuery] MatchRequest matchRequest)
    {
        if (matchRequest is null)
            return BadRequest("Match data is null.");

        var match = new Match
        {
            HomeTeamName = matchRequest.HomeTeamName,
            AwayTeamName = matchRequest.AwayTeamName,
            Round = (int)matchRequest.Round,
            Competition = matchRequest.Competition
        };

        if (!string.IsNullOrEmpty(matchRequest.Score))
        {           
            var goals = ParseScore(matchRequest.Score);
            if (goals is not null)
            {
                var (homeGoals, awayGoals) = goals.Value;
                match.HomeTeamGoals = homeGoals;
                match.AwayTeamGoals = awayGoals;
            }
        }
        else
        {
            match.HomeTeamGoals = 99;
            match.AwayTeamGoals = 99;
        }

        var result = await matchRepository.AddMatch(match);
        if (!result)
            return StatusCode(500, "Could not insert match into DB");

        result = await teamRepository.UpdateTeamPoints(match);
        if (!result)
            return StatusCode(500, "Could not update match's teams points");

        return Ok("Match added successfully.");
    }

    [HttpPut]
    [SwaggerOperation(Tags = new[] { "Matches - Put" })]
    public async Task<ActionResult> UpdateMatch([FromQuery] MatchRequest matchRequest)
    {
        if (matchRequest is null || string.IsNullOrEmpty(matchRequest.Score))
        {
            return BadRequest("Match data like score is null.");
        }

        var goals = ParseScore(matchRequest.Score!);
        if (goals is null)
            return BadRequest("Match format is not ok.");
        var (homeGoals, awayGoals) = goals.Value;

        var match = new Match
        {
            HomeTeamName = matchRequest.HomeTeamName,
            AwayTeamName = matchRequest.AwayTeamName,
            Round = (int)matchRequest.Round,
            HomeTeamGoals = homeGoals,
            AwayTeamGoals = awayGoals,
            Competition = matchRequest.Competition,
            Id = matchRequest.Id
        };
        var result = await matchRepository.UpdateMatch(match, match.Id);
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

    private static (int, int)? ParseScore(string score)
    {
        string[] matchResultParts = score.Split('-', StringSplitOptions.TrimEntries);
        if (matchResultParts.Length != 2 || !int.TryParse(matchResultParts[0], out var homeGoals) || !int.TryParse(matchResultParts[1], out var awayGoals) || homeGoals < 0 || awayGoals < 0)
        {
            return null;
        }

        return (homeGoals, awayGoals);
    }
}
