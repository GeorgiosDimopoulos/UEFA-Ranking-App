using Dapper;
using Microsoft.Data.Sqlite;

namespace Core.DataAccess;

public class CountryRepository : ICountryRepository
{
    private readonly string _connectionString;

    public CountryRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<Country>> GetAllCountries()
    {
        using var connection = new SqliteConnection(_connectionString);

        var countries = await connection.QueryAsync<Country>("SELECT Id, Name, Position FROM Countries ORDER BY Position ASC");
        return countries.ToList();
    }

    public async Task<Country?> GetCountryById(int id)
    {
        if (id <= 0)
            return null;

        using var connection = new SqliteConnection(_connectionString);

        var country = await connection.QuerySingleOrDefaultAsync<Country>("SELECT * FROM Countries WHERE Id = @Id", new { Id = id });
        return country;
    }

    public async Task<Country?> GetCountryByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        using var connection = new SqliteConnection(_connectionString);

        var country = await connection.QuerySingleOrDefaultAsync<Country>("SELECT * FROM Countries WHERE Name = @Name", new { Name = name });
        return country;
    }

    public async Task AddCountry(Country c)
    {
        using var connection = new SqliteConnection(_connectionString);

        var newCountry = new Country
        {
            Name = c.Name,
            Position = c.Position
        };

        var insertCountryQuery =;
    }

    public async Task UpdateCountry(Country c)
    {
        using var connection = new SqliteConnection(_connectionString);

    }

    public async Task DeleteCountry(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

    }
}
