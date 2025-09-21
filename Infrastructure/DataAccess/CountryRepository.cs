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

        var countries = await connection.QueryAsync<Country>("SELECT Id, Name, TotalPoints, Position, NumberOfActiveTeams FROM Countries ORDER BY Position ASC");
        return countries.ToList();
    }

    public async Task<Country?> GetCountryById(int id)
    {
        if (id <= 0)
            return null;

        using var connection = new SqliteConnection(_connectionString);

        var country = await connection.QuerySingleOrDefaultAsync<Country>("SELECT Id, Name, Position, TotalPoints FROM Countries WHERE Id = @Id", new { Id = id });
        return country;
    }

    public async Task<Country?> GetCountryByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        using var connection = new SqliteConnection(_connectionString);

        var country = await connection.QuerySingleOrDefaultAsync<Country>("SELECT Id, Name, Position, TotalPoints FROM Countries WHERE Name = @Name", new { Name = name });
        return country;
    }

    public async Task<bool> AddCountry(Country c)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();        

        var availableCountry = await connection.QuerySingleOrDefaultAsync<Country>("SELECT * FROM Countries WHERE Name = @Name", new { c.Name });
        if (availableCountry != null)
        {
            return false;
        }

        var newCountry = new Country
        {
            Name = c.Name,
            TotalPoints = c.TotalPoints,
        };

        var countries = (await GetAllCountries()).OrderByDescending(c => c.TotalPoints);
        int position = 1;
        foreach (var dbCountry in countries)
        {
            if (c.TotalPoints < dbCountry.TotalPoints)
                position++;
            else
                break;
        }
        newCountry.Position = position;

        var insertCountryQuery = @"INSERT INTO Countries (Name, Position, TotalPoints) VALUES(@Name, @Position, @TotalPoints)";
        var result = await connection.ExecuteAsync(insertCountryQuery, newCountry);

        Console.WriteLine($"New country added: {newCountry.Name}");
        return result > 0;
    }

    public async Task<bool> UpdateCountry(Country c, int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        var updateCountryQuery = "UPDATE Countries SET Name = @Name, Position = @Position, @TotalPoints = TotalPoints WHERE Id = @Id";

        var result = await connection.ExecuteAsync(updateCountryQuery, new { c.Name, c.Position, Id = id, c.TotalPoints});
        return result > 0;
    }

    public async Task<bool> DeleteCountry(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        var deleteTeamsQuery = "DELETE FROM Teams WHERE CountryId = @CountryId";
        await connection.ExecuteAsync(deleteTeamsQuery, new { CountryId = id });

        var deleteCountryQuery = "DELETE FROM Countries WHERE Id = @Id";

        var result = await connection.ExecuteAsync(deleteCountryQuery, new { Id = id });
        return result > 0;
    }

    public async Task<bool> DeleteCountryByName(string name)
    {
        using var connection = new SqliteConnection(_connectionString);

        var countryId = "SELECT Id FROM Countries WHERE Id = @Id";
        var deleteTeamsQuery = "DELETE FROM Teams WHERE CountryId = @countryId";
        await connection.ExecuteAsync(deleteTeamsQuery, new { CountryId = countryId });

        var deleteCountryQuery = "DELETE FROM Countries WHERE Id = @countryId";

        var result = await connection.ExecuteAsync(deleteCountryQuery, new { Id = name });
        return result > 0;
    }

    public async Task<bool> DeleteCountries()
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = await connection.ExecuteAsync("DELETE FROM Teams");
        var result2 = await connection.ExecuteAsync("DELETE FROM Countries");
        return result > 0 && result2 > 0;
    }
}
