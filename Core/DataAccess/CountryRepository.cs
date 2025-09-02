namespace Core.DataAccess;

public class CountryRepository : ICountryRepository
{
    private readonly string _connectionString;

    public CountryRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task AddCountry(Country c)
    {
        using var connection = new SqlConnection(_connectionString);
    }

    public Task DeleteCountry(int id)
    {
    }

    public Task<List<Country>> GetAllCountries()
    {
    }

    public Task<Country?> GetCountryById(int id)
    {
    }

    public Task<Country?> GetCountryByName(string name)
    {
    }

    public Task UpdateCountry(Country c)
    {
    }
}
