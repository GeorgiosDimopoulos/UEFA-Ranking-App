using Core.QueryParameters;

namespace Core.Interfaces;

public interface ICountryRepository
{
    public Task<List<Country>> GetAllCountries(CountryQueryParameters cqueryParameters);
    public Task<Country?> GetCountryById(int id, CountryQueryParameters cqueryParameters);
    public Task<Country?> GetCountryByName(string name, CountryQueryParameters cqueryParameters);

    public Task<Dictionary<string, int>> GetCountriesNamesAndPoints(CountryQueryParameters cqueryParameters);
    public Task<bool> AddCountry(Country c);
    public Task<bool> UpdateCountry(Country c, int id);
    public Task<bool> DeleteCountry(int id);
    public Task<bool> DeleteCountryByName(string n);
    public Task<bool> DeleteCountries();
}
