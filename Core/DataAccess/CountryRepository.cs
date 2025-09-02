using Dapper;
using Microsoft.Data.Sqlite;

namespace Core.DataAccess;

public class CountryRepository : ICountryRepository
{
    private readonly string _connectionString;

    public CountryRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string is not set.");

        _connectionString = connectionString;
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

    public async Task AddCountry(Country c)
    {
        using var connection = new SqliteConnection(_connectionString);

        var newCountry = new Country
        {
            Name = c.Name,
            Position = c.Position
        };

        var insertCountryQuery = "INSERT INTO Countries (Name, Position) VALUES(@Name, @Position)";
        await connection.ExecuteAsync(insertCountryQuery, newCountry);
    }

    public async Task UpdateCountry(Country c)
    {
        using var connection = new SqliteConnection(_connectionString);

        var updateCountryQuery = "UPDATE Countries SET Name = @Name, Position = @Position WHERE Id = @Id";
        var result = await connection.ExecuteAsync(updateCountryQuery, c);
        if (result == 0)
            throw new InvalidOperationException("Country does not exist in the database.");
    }

    public async Task DeleteCountry(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        var deleteCountryQuery = "DELETE FROM Countries WHERE Id = @Id";

        var result = await connection.ExecuteAsync(deleteCountryQuery, new { Id = id });
        if (result == 0)
            throw new InvalidOperationException("Country does not exist in the database.");
    }
}
