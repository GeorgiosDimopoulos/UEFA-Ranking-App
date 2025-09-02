using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class CountriesController : ControllerBase
{
    private readonly ILogger<TeamsController> _logger;

    public CountriesController(ILogger<TeamsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<Country> Get()
    {
        return [.. Enumerable.Range(1, 5).Select(index => new Country
        {
        })];
    }
}
