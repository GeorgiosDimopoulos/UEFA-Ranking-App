using Core.Data.Models;
using Core.DataAccess.Interfaces;

namespace Core.DataAccess;

public class CountryRepository : ICountryRepository
{
    public Task AddCountry(Country c)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCountry(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Country>> GetAllCountries()
    {
        throw new NotImplementedException();
    }

    public Task<Country?> GetCountryById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Country?> GetCountryByName(string name)
    {
        throw new NotImplementedException();
    }

    public Task UpdateCountry(Country c)
    {
        throw new NotImplementedException();
    }
}
