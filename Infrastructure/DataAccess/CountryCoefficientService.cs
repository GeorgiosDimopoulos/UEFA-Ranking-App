namespace Infrastructure.DataAccess;

public class CountryCoefficientService : ICountryCoefficientService
{
    public async Task<double> GetCountryPoints(string country)
    {
    }

    public async Task<bool> UpdateCountryCoefficient(string country, int countryNewPoints)
    {
    }

    public async Task<bool> RecalculateForMatchAsync(int matchId)
    {
    }
}
