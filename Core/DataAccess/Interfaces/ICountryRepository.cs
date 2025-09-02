namespace Core.DataAccess.Interfaces;

public interface ICountryRepository
{
    public Task<List<Country>> GetAllCountries();
    public Task<Country?> GetCountryById(int id);
    public Task<Country?> GetCountryByName(string name);
    public Task AddCountry(Country c);
    public Task UpdateCountry(Country c);
    public Task DeleteCountry(int id);
}
