namespace Infrastructure.DataAccess;

public class CountryCoefficientService : ICountryCoefficientService
{
    public async Task<double> GetCountryPoints(string country)
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
