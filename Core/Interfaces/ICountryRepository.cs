using Core.Models;

namespace Core.Interfaces;

public interface ICountryRepository
{
    public Task<List<Country>> GetAllCountries();
    public Task<Country?> GetCountryById(int id);
    public Task<Country?> GetCountryByName(string name);
    public Task<bool> AddCountry(Country c);
    public Task<bool> UpdateCountry(Country c);
    public Task<bool> DeleteCountry(int id);
}
