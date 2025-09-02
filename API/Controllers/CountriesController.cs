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

    [HttpGet(Name = "CountryById")]
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
    public async Task AddCountry(Country c)
    {
        await countryRepository.AddCountry(c);
    }

    [HttpPut(Name = "UpdateCountry")]
    public async Task UpdateCountry(Country c)
    {
        await countryRepository.UpdateCountry(c);
    }

    [HttpDelete(Name = "DeleteCountry")]
    public async Task DeleteCountry(int id)
    {
        await countryRepository.DeleteCountry(id);
    }
}
