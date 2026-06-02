using FluentResults;
using Core.QueryParameters;

namespace Core.Interfaces;

public interface ICountryRepository
{
    public Task<List<Country>> GetAllCountries(CountryQueryParameters cqueryParameters);
    public Task<Country?> GetCountryById(int id, CountryQueryParameters cqueryParameters);
    public Task<Country?> GetCountryByName(string name, CountryQueryParameters cqueryParameters);

    public Task<Dictionary<string, int>> GetCountriesNamesAndPoints(CountryQueryParameters cqueryParameters);
    public Task<Result<Country>> AddCountry(Country c);
    public Task<Result> UpdateCountry(Country c, int id);
    public Task<Result> DeleteCountry(int id);
    public Task<Result> DeleteCountryByName(string n);
    public Task<Result> DeleteCountries();
}
