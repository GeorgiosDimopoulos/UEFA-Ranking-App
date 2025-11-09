
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

    public async Task<bool> UpdateCountryCoefficient(int countryId, int matchResult)
    {
        var country = await countryRepository.GetCountryById(countryId);
        if (country is null)
        {
            return false;
        }

        int countryNewPoints = await CalculateCountryMatch(country, matchResult);

        var countryNewTotalPoints = country.TotalPoints + countryNewPoints;

        // ToDo: calculate new points due to the number of the teams of the country devided by initial teams        

        return await countryRepository.UpdateCountry(country, countryNewTotalPoints);
    }

    public async Task<bool> RecalculateForMatchAsync(int matchId)
    {
        throw new NotImplementedException();
    }

    private async Task<int> CalculateCountryMatch(Country country, int matchResult)
    {
        int countryInitialTeams = (await teamRepository.GetTeamsByCountryId(country.Id)).Count;
        switch (matchResult)
        {
            case 1: // win
                return 2000/ countryInitialTeams;
            case 2: // draw
                return 1000 / countryInitialTeams;
            case 3: // loss
                return 0;
            default:
                break;
        }
        return 0;
    }
}
