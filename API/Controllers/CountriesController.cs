using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly ILogger<TeamsController> _logger;
    private readonly ICountryRepository countryRepository;

    public CountriesController(ILogger<TeamsController> logger, ICountryRepository countryRepository)
    {
        _logger = logger;
        this.countryRepository = countryRepository;
    }

    [HttpGet(Name = "Countries")]
    public async Task<IEnumerable<Country>> GetCountries()
    {
        return await countryRepository.GetAllCountries();
    }

    [HttpGet("{$id:int}", Name = "CountryById")]
    public async Task<Country> GetCountryById(int id)
    {
        return await countryRepository.GetCountryById(id) ?? new();
    }

    [HttpGet(Name = "CountryByName")]
    public async Task<Country> GetCountryByName(string n)
    {
        return await countryRepository.GetCountryByName(n) ?? new();
    }

    [HttpPost(Name = "AddCountry")]
    public async Task<ActionResult> AddCountry([FromBody] Country c)
    {
        var result = await countryRepository.AddCountry(c);
        if (result == false)
        {
            _logger.LogWarning("Could not add country {Country}", c.Name);
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetCountryById), new { id = c.Id }, c);
    }

    [HttpPut("{id: int}", Name = "UpdateCountry")]
    public async Task<ActionResult> UpdateCountry(Country c, int id)
    {
        c.Id = id;
        var result = await countryRepository.UpdateCountry(c);
        if (result == false)
        {
            _logger.LogWarning("Could not update country {Country}", c.Name);
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id: int}", Name = "DeleteCountry")]
    public async Task<ActionResult> DeleteCountry(int id)
    {
        var result = await countryRepository.DeleteCountry(id);
        if (result == false)
        {
            _logger.LogWarning("Could not delete country with id {Id}", id);
            return NotFound();
        }

        return BadRequest();
    }
}
