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
    public async Task<IEnumerable<Country>> GetCountries()
    {
        return await countryRepository.GetAllCountries();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Country>> GetCountryById(int id)
    {
        var country = await countryRepository.GetCountryById(id) ?? new();
        if (country == null)
        {
            _logger.LogWarning("Country with id {Id} not found", id);
            return NotFound();
        }

        return Ok(country);
    }

    [HttpGet("byName")]
    public async Task<ActionResult<Country>> GetCountryByName(string n)
    {
        var country = await countryRepository.GetCountryByName(n) ?? new();
        if (country == null)
        {
            _logger.LogWarning("Country with name {Name} not found", n);
            return NotFound();
        }

        return Ok(country);
    }

    [HttpPost()]
    public async Task<ActionResult> AddCountry([FromBody] Country c)
    {
        var result = await countryRepository.AddCountry(c);
        if (result == false)
        {
            _logger.LogWarning("Could not add country {Country}", c.Name);
            return BadRequest();
        }

        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateCountry(Country c, int id)
    {
        c.Id = id;
        var result = await countryRepository.UpdateCountry(c);
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
