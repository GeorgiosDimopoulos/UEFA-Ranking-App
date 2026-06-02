using API.Data.DTOs;
using Core.Interfaces;
using Core.Models;
using Core.QueryParameters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Countries")]
public class CountriesController : ControllerBase
{
    private readonly ILogger<CountriesController> _logger;
    private readonly ICountryRepository countryRepository;
    private readonly ITeamRepository teamRepository;

    public CountriesController(ILogger<CountriesController> logger, ICountryRepository countryRepository, ITeamRepository teamRepository)
    {
        _logger = logger;
        this.countryRepository = countryRepository;
        this.teamRepository = teamRepository;
    }

    [HttpGet(Name = "Countries")]
    [SwaggerOperation(Tags = new[] { "Countries - Get" })]
    public async Task<IEnumerable<CountryResponse>> GetCountries([FromQuery] CountryQueryParameters cqueryParameters, [FromQuery] TeamQueryParameters tqueryParameters)
    {
        var countries = await countryRepository.GetAllCountries(cqueryParameters);
        var teams = await teamRepository.GetAllTeams(tqueryParameters);

        var countriesPositions = countries.Select(c => c.TotalPoints)
                                          .Distinct()
                                          .OrderByDescending(p => p)
                                          .Select((p, i) => new { p, pos = i + 1 })
                                          .ToDictionary(x => x.p, x => x.pos);

        return countries.Select(c => new CountryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Position = countriesPositions[c.TotalPoints],
            NumberOfInitialTeams = teams.Count(t => t.CountryId == c.Id),
            NumberOfActiveTeams = teams.Count(t => t.CountryId == c.Id && t.IsActive),
            TotalPoints = c.TotalPoints
        });
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Tags = new[] { "Countries - Get" })]
    public async Task<ActionResult<CountryResponse>> GetCountryById(int id, [FromQuery] TeamQueryParameters tqueryParameters, [FromQuery] CountryQueryParameters cqueryParameters)
    {
        var country = await countryRepository.GetCountryById(id, cqueryParameters);
        if (country == null)
        {
            _logger.LogWarning($"Country with id {id} not found");
            return NotFound();
        }

        var countryTeams = await teamRepository.GetTeamsByCountryId(country.Id, tqueryParameters);

        var countryDto = new CountryResponse
        {
            Id = country.Id,
            Name = country.Name,
            Position = await GetCountryPosition(country.TotalPoints),
            NumberOfInitialTeams = countryTeams?.Count() ?? 0,
            NumberOfActiveTeams = countryTeams?.Where(t => t.IsActive).Count() ?? 0,
            TotalPoints = country.TotalPoints
        };

        return Ok(countryDto);
    }

    [HttpGet("by-name/{name}")]
    [SwaggerOperation(Tags = new[] { "Countries - Get" })]
    public async Task<ActionResult<CountryResponse>> GetCountryByName(string name, [FromQuery] CountryQueryParameters cqueryParameters)
    {
        var country = await countryRepository.GetCountryByName(name, cqueryParameters);
        if (country == null)
        {
            _logger.LogWarning($"Country with name {name} not found");
            return NotFound();
        }

        var countryTeams = await teamRepository.GetTeamsByCountryId(country.Id, new() { IncludeMatches = cqueryParameters.IncludeMatches, IncludeCountry = false });

        var countryDto = new CountryResponse
        {
            Id = country.Id,
            Name = country.Name,
            Position = await GetCountryPosition(country.TotalPoints),
            NumberOfInitialTeams = countryTeams?.Count() ?? 0,
            NumberOfActiveTeams = countryTeams?.Where(t => t.IsActive).Count() ?? 0,
            TotalPoints = country.TotalPoints
        };

        return Ok(countryDto);
    }

    [HttpPost()]
    [SwaggerOperation(Tags = new[] { "Countries – Post" })]
    public async Task<ActionResult> AddCountry([FromQuery] CountryRequest c)
    {
        var country = new Country { Name = c.Name, TotalPoints = c.TotalPoints };
        var result = await countryRepository.AddCountry(country);
        if (result.IsFailed)
        {
            _logger.LogWarning($"Could not add country {c.Name}");
            return BadRequest();
        }

        return Ok();
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Tags = new[] { "Countries – Put" })]
    public async Task<ActionResult> UpdateCountry([FromQuery] CountryRequest c, int id)
    {
        var country = new Country { Name = c.Name, TotalPoints = c.TotalPoints };
        var result = await countryRepository.UpdateCountry(country, id);
        if (result.IsFailed)
        {
            _logger.LogWarning($"Could not update country {c.Name}");
            return NotFound();
        }

        return Ok();
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Tags = new[] { "Countries – Delete" })]
    public async Task<ActionResult> DeleteCountry(int id)
    {
        var countryTeams = await teamRepository.GetTeamsByCountryId(id, null!);
        if (countryTeams.Count > 0)
        {
            foreach (var team in countryTeams)
            {
                await teamRepository.DeleteTeamByName(team!.Name);
            }
        }

        var result = await countryRepository.DeleteCountry(id);
        if (result.IsFailed)
        {
            _logger.LogWarning($"Could not delete country with id {id}");
            return NotFound();
        }

        return NoContent();
    }

    //[HttpDelete("{n}")]
    //[SwaggerOperation(Tags = new[] { "Countries – Delete" })]
    //public async Task<ActionResult> DeleteCountry(string n)
    //{
    //    var countryTeams = await teamRepository.GetTeams(id);
    //    if (countryTeams.Count > 0)
    //    {
    //        foreach (var team in countryTeams)
    //        {
    //            await teamRepository.DeleteTeamByName(team!.Name);
    //        }
    //    }

    //    var result = await countryRepository.DeleteCountryByName(n);
    //    if (result == false)
    //    {
    //        _logger.LogWarning($"Could not delete country with name {n}");
    //        return NotFound();
    //    }

    //    return NoContent();
    //}

    [HttpDelete]
    [SwaggerOperation(Tags = new[] { "Countries – Delete" })]
    public async Task<ActionResult> DeleteCountries()
    {
        var result = await countryRepository.DeleteCountries();
        if (result.IsFailed)
        {
            _logger.LogWarning("Could not delete countries");
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("points/{countryId}")]
    [SwaggerOperation(Tags = new[] { "Countries - Get" })]
    public async Task<int> GetCountryPoints(int countryId)
    {
        var country = await countryRepository.GetCountryById(countryId, new() { IncludeMatches = false, IncludeTeams = false });
        if (country is null)
        {
            return 0;
        }
        return country.TotalPoints;
    }

    [HttpPut("update-coefficient/{countryId}/{matchResult}")]
    [SwaggerOperation(Tags = new[] { "Countries – Put" })]
    public async Task<bool> UpdateCountryCoefficient(int countryId, int matchResult)
    {
        var country = await countryRepository.GetCountryById(countryId, null!);
        if (country is null)
        {
            return false;
        }

        int countryNewPoints = await CalculateCountryMatch(country, matchResult);

        var countryNewTotalPoints = country.TotalPoints + countryNewPoints;

        var result = await countryRepository.UpdateCountry(country, countryNewTotalPoints);
        if (result.IsFailed)
        {
            _logger.LogWarning($"Could not update country with id {countryId}");
            return false;
        }

        return true;
    }

    private async Task<int> CalculateCountryMatch(Country country, int matchResult)
    {
        int countryInitialTeams = (await teamRepository.GetTeamsByCountryId(country.Id, null!)).Count;
        switch (matchResult)
        {
            case 1: // win
                return 2000 / countryInitialTeams;
            case 2: // draw
                return 1000 / countryInitialTeams;
            case 3: // loss
                return 0;
            default:
                break;
        }
        return 0;
    }

    private async Task<int> GetCountryPosition(int points)
    {
        var teams = await countryRepository.GetCountriesNamesAndPoints(new());
        var orderedTeams = teams.OrderByDescending(t => t.Value).ToList();
        var teamPosition = orderedTeams.FindIndex(t => t.Value == points) + 1;
        return teamPosition;
    }
}