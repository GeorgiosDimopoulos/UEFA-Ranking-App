namespace Core.Interfaces;

public interface ICountryCoefficientService
{
    Task<int> GetCountryPoints(int countryId);
    Task<bool> UpdateCountryCoefficient(int countryId, int countryNewPoints);
    Task<bool> RecalculateForMatchAsync(int matchId);
}
