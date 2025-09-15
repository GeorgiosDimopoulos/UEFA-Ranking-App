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

    public CountriesController(ILogger<CountriesController> logger, ICountryRepository countryRepository)
    {
        _logger = logger;
        this.countryRepository = countryRepository;
    }

    [HttpGet(Name = "Countries")]
    public async Task<IEnumerable<CountryDto>> GetCountries()
    {
        var countries = await countryRepository.GetAllCountries();
        return countries.Select(c => new CountryDto
        {
            Name = c.Name,
            Position = c.Position,
            NumberOfTeams = c.Teams?.Count() ?? 0,
            TotalPoints = c.Teams?.Sum(t => t.Points) ?? 0
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CountryDto>> GetCountryById(int id)
    {
        var country = await countryRepository.GetCountryById(id) ?? new();
        if (country == null)
        {
            _logger.LogWarning("Country with id {Id} not found", id);
            return NotFound();
        }

        var countryDto = new CountryDto
        {
            Name = country.Name,
            Position = country.Position,
            NumberOfTeams = country.Teams?.Count() ?? 0,
            TotalPoints = country.Teams?.Sum(t => t.Points) ?? 0
        };

        return Ok(countryDto);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<CountryDto>> GetCountryByName(string name)
    {
        var country = await countryRepository.GetCountryByName(name) ?? new();
        if (country == null)
        {
            _logger.LogWarning("Country with name {name} not found", name);
            return NotFound();
        }

        var countryDto = new CountryDto
        {
            Name = country.Name,
            Position = country.Position,
            NumberOfTeams = country.Teams?.Count() ?? 0,
            TotalPoints = country.Teams?.Sum(t => t.Points) ?? 0
        };

        return Ok(countryDto);
    }

    [HttpPost()]
    public async Task<ActionResult> AddCountry([FromQuery] CountryDto c)
    {
        var country = new Country { Name = c.Name, Position = c.Position };
        var result = await countryRepository.AddCountry(country);
        if (result == false)
        {
            _logger.LogWarning("Could not add country {Country}", c.Name);
            return BadRequest();
        }

        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateCountry([FromQuery] CountryDto c, int id)
    {
        var country = new Country { Name = c.Name, Position = c.Position };
        var result = await countryRepository.UpdateCountry(country, id);
        if (result == false)
        {
            _logger.LogWarning("Could not update country {Country}", c.Name);
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteCountry(int id)
    {
        var result = await countryRepository.DeleteCountry(id);
        if (result == false)
        {
            _logger.LogWarning("Could not delete country with id {Id}", id);
            return NotFound();
        }

        return NoContent();
    }
}
