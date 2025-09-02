using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DataAccess;

public class CountryRepository : ICountryRepository
{
    private readonly string _connectionString;

    public CountryRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string is not set.");
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Connection string is not set.");
    }

    public async Task<List<Country>> GetAllCountries()
    {
        using var connection = new SqliteConnection(_connectionString);

        var countries = await connection.QueryAsync<Country>("SELECT * FROM Countries ORDER BY Position ASC");
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

    public async Task<bool> AddCountry(Country c)
    {
        using var connection = new SqliteConnection(_connectionString);

        var newCountry = new Country
        {
            Name = c.Name,
            Position = c.Position
        };

        var insertCountryQuery = "INSERT INTO Countries (Name, Position) VALUES(@Name, @Position)";
        var result = await connection.ExecuteAsync(insertCountryQuery, newCountry);
        return result > 0;
    }

    public async Task<bool> UpdateCountry(Country c)
    {
        using var connection = new SqliteConnection(_connectionString);

        var updateCountryQuery = "UPDATE Countries SET Name = @Name, Position = @Position WHERE Id = @Id";

        var result = await connection.ExecuteAsync(updateCountryQuery, c);
        return result > 0;
    }

    public async Task<bool> DeleteCountry(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        var deleteCountryQuery = "DELETE FROM Countries WHERE Id = @Id";

        var result = await connection.ExecuteAsync(deleteCountryQuery, new { Id = id });
        return result > 0;
    }
}
