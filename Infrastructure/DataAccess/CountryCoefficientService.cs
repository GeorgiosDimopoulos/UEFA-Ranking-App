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

    public async Task<int> GetCountryPoints(int countryId)
    {
        var country = await countryRepository.GetCountryById(countryId);
        if (country is null)
        {
            return 0;
        }
        return country.TotalPoints;
    }

    public async Task<bool> UpdateCountryCoefficient(int countryId, int countryNewPoints)
    {
        var country = await countryRepository.GetCountryById(countryId);
        if (country is null)
        {
            return false;
        }

        var countryNewTotalPoints = country.TotalPoints + countryNewPoints;

        // ToDo: calculate new points due to the number of the teams of the country devided by initial teams

        return await countryRepository.UpdateCountry(country, countryNewTotalPoints);
    }

    public async Task<bool> RecalculateForMatchAsync(int matchId)
    {
        throw new NotImplementedException();
    }
}
