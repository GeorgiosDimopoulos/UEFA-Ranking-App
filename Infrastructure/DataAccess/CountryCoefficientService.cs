  namespace Infrastructure.DataAccess;

public class CountryCoefficientService : ICountryCoefficientService
{
    private readonly ITeamRepository teamRepository;
    private readonly ICountryRepository countryRepository;
    private readonly IMatchRepository matchRepository;

    public CountryCoefficientService(ITeamRepository teamRepository, ICountryRepository countryRepository, IMatchRepository matchRepository)
    {            
        this.teamRepository = teamRepository;
        this.countryRepository = countryRepository;
        this.matchRepository = matchRepository;
    }

    public async Task<int> GetCountryPoints(string country)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateCountryCoefficient(string country, int countryNewPoints)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RecalculateForMatchAsync(int matchId)
    {
        throw new NotImplementedException();
    }
}
