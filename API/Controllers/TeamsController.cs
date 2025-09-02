using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(ILogger<TeamsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<Team> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new Team
        {
        })
        .ToArray();
    }
}
