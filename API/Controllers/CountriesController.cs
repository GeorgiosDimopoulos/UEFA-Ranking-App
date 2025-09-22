using API.Data.DTOs;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IEnumerable<CountryResponse>> GetCountries()
    {
        var countries = await countryRepository.GetAllCountries();
        var teams = await teamRepository.GetAllTeams();

        var teamsByCountry = teams.GroupBy(t => t.CountryId)
                                  .ToDictionary(g => g.Key, g => g.ToList());

        return countries.Select(c => new CountryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Position = c.Position,
            NumberOfInitialTeams = teams.Count(t => t.CountryId == c.Id),
            NumberOfActiveTeams = teams.Count(t => t.CountryId == c.Id && t.IsActive),
            TotalPoints = c.TotalPoints
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CountryResponse>> GetCountryById(int id)
    {
        var country = await countryRepository.GetCountryById(id) ?? new();
        if (country == null)
        {
            _logger.LogWarning($"Country with id {id} not found");
            return NotFound();
        }

        var countryTeams = await teamRepository.GetTeamsByCountryId(country.Id);

        var countryDto = new CountryResponse
        {
            Id = country.Id,
            Name = country.Name,
            Position = country.Position,
            NumberOfInitialTeams = countryTeams?.Count() ?? 0,
            NumberOfActiveTeams = countryTeams?.Where(t => t.IsActive).Count() ?? 0,
            TotalPoints = countryTeams?.Sum(t => t.Points) ?? 0
        };

        return Ok(countryDto);
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<CountryResponse>> GetCountryByName(string name)
    {
        var country = await countryRepository.GetCountryByName(name) ?? new();
        if (country == null)
        {
            _logger.LogWarning($"Country with name {name} not found");
            return NotFound();
        }

        var countryTeams = await teamRepository.GetTeamsByCountryId(country.Id);

        var countryDto = new CountryResponse
        {
            Id = country.Id,
            Name = country.Name,
            Position = country.Position,
            NumberOfInitialTeams = countryTeams?.Count() ?? 0,
            NumberOfActiveTeams = countryTeams?.Where(t => t.IsActive).Count() ?? 0,
            TotalPoints = countryTeams?.Sum(t => t.Points) ?? 0
        };

        return Ok(countryDto);
    }

    [HttpPost()]
    public async Task<ActionResult> AddCountry(string name, int points)
    {
        var country = new Country { Name = name, TotalPoints = points };
        var result = await countryRepository.AddCountry(country);
        if (result == false)
        {
            _logger.LogWarning($"Could not add country {name}");
            return BadRequest();
        }

        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateCountry([FromQuery] CountryRequest c, int id)
    {
        var country = new Country { Name = c.Name};
        var result = await countryRepository.UpdateCountry(country, id);
        if (result == false)
        {
            _logger.LogWarning($"Could not update country {c.Name}");
            return NotFound();
        }

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteCountry(int id)
    {
        var result = await countryRepository.DeleteCountry(id);
        if (result == false)
        {
            _logger.LogWarning($"Could not delete country with id {id}");
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("by-name/{name}")]
    public async Task<ActionResult> DeleteCountryByName(string n)
    {
        var result = await countryRepository.DeleteCountryByName(n);
        if (result == false)
        {
            _logger.LogWarning($"Could not delete country with name {n}");
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCountries()
    {
        var result = await countryRepository.DeleteCountries();
        if (result == false)
        {
            _logger.LogWarning("Could not delete some countries");
            return NotFound();
        }

        return NoContent();
    }
}