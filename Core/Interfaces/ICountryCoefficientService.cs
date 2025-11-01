namespace Core.Interfaces;

public interface ICountryCoefficientService
{
    Task<int> GetCountryPoints(string country);
    Task<bool> UpdateCountryCoefficient(string country, int countryNewPoints);
    Task<bool> RecalculateForMatchAsync(int matchId);
}
