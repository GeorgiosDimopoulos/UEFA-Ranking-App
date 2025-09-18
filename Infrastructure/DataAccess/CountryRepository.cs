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

        var countries = await connection.QueryAsync<Country>("SELECT Id, Name, TotalPoints, NumberOfActiveTeams FROM Countries ORDER BY Position ASC");
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

        await connection.OpenAsync();
        Console.WriteLine($"[SQLite] Opened DB file: {connection.DataSource}");

        await using var tx = await connection.BeginTransactionAsync();

        var currentCountriesNumber = (await GetAllCountries()).Count;

        var availableCountry = await connection.QuerySingleOrDefaultAsync<Country>("SELECT * FROM Countries WHERE Name = @Name", new { c.Name });
        if (availableCountry != null)
        {
            await tx.RollbackAsync();
            return false;
        }

        var newCountry = new Country
        {
            Name = c.Name,
            TotalPoints = c.TotalPoints,
            Teams = []
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

        return result > 0;
    }

    public async Task<bool> UpdateCountry(Country c, int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        var updateCountryQuery = "UPDATE Countries SET Name = @Name, Position = @Position, NumberOfInitialTeams= @NumberOfInitialTeams WHERE Id = @Id";

        var result = await connection.ExecuteAsync(updateCountryQuery, new { c.Name, c.Position, Id = id, c.TotalPoints, c.NumberOfInitialTeams });
        return result > 0;
    }

    public async Task<bool> DeleteCountry(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        var deleteTeamsQuery = "DELETE FROM Teams WHERE CountryId = @CountryId";
        var result = await connection.ExecuteAsync(deleteTeamsQuery, new { CountryId = id });

        var deleteCountryQuery = "DELETE FROM Countries WHERE Id = @Id";

        var result2 = await connection.ExecuteAsync(deleteCountryQuery, new { Id = id });
        return result > 0 && result2 > 0;
    }

    public async Task<bool> DeleteCountries()
    {
        using var connection = new SqliteConnection(_connectionString);

        var result = await connection.ExecuteAsync("DELETE FROM Teams");
        var result2 = await connection.ExecuteAsync("DELETE FROM Countries");
        return result > 0 && result2 > 0;
    }
}
