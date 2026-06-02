using Core.QueryParameters;
using Dapper;
using FluentResults;
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

    public async Task<List<Country>> GetAllCountries(CountryQueryParameters cqueryParameters)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        if (cqueryParameters.IncludeTeams)
        {

        }
        if (cqueryParameters.IncludeMatches)
        {

        }

        var countries = await connection.QueryAsync<Country>("SELECT Id, Name, TotalPoints, NumberOfActiveTeams, NumberOfInitialTeams FROM Countries");
        return countries.ToList();
    }

    public async Task<Dictionary<string, int>> GetCountriesNamesAndPoints(CountryQueryParameters cqueryParameters)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        if (cqueryParameters.IncludeTeams)
        {

        }
        if (cqueryParameters.IncludeMatches)
        {

        }

        var countriesNamesAndPoints = await connection.QueryAsync<(string Name, int Points)>("SELECT Name, TotalPoints FROM Countries");
        return countriesNamesAndPoints.ToDictionary(c => c.Name, c => c.Points);
    }

    public async Task<Country?> GetCountryById(int id, CountryQueryParameters cqueryParameters)
    {
        if (id <= 0)
            return null;

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var country = await connection.QuerySingleOrDefaultAsync<Country>("SELECT Id, Name, TotalPoints FROM Countries WHERE Id = @Id", new { Id = id });
        return country;
    }

    public async Task<Country?> GetCountryByName(string name, CountryQueryParameters cqueryParameters)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        if (cqueryParameters.IncludeTeams)
        {

        }
        if (cqueryParameters.IncludeMatches)
        {

        }

        var country = await connection.QuerySingleOrDefaultAsync<Country>("SELECT Id, Name, TotalPoints FROM Countries WHERE Name = @Name", new { Name = name });
        return country;
    }

    public async Task<Result<Country>> AddCountry(Country c)
    {
        if (string.IsNullOrWhiteSpace(c.Name))
            return Result.Fail<Country>("Country name is required.");

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var availableCountry = await connection.QuerySingleOrDefaultAsync<Country>("SELECT * FROM Countries WHERE Name = @Name", new { c.Name });
            if (availableCountry != null)
            {
                return Result.Fail<Country>("A country with the same name already exists.");
            }

            var newCountry = new Country
            {
                Name = c.Name,
                TotalPoints = c.TotalPoints,
            };

            var insertCountryQuery = @"INSERT INTO Countries (Name, TotalPoints) VALUES (@Name, @TotalPoints)";
            var result = await connection.ExecuteAsync(insertCountryQuery, new { newCountry.Name, newCountry.TotalPoints });

            if (result <= 0)
            {
                return Result.Fail<Country>("Failed to add the country.");
            }

            return Result.Ok(newCountry).WithSuccess("Country added successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail<Country>($"An error occurred while adding the country: {ex.Message}");
        }
    }

    public async Task<Result> UpdateCountry(Country c, int id)
    {
        if (string.IsNullOrWhiteSpace(c.Name))
            return Result.Fail("Country name is required.");
        if (id <= 0)
            return Result.Fail("Country Id must be greater than 0.");

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            var updateCountryQuery = "UPDATE Countries SET Name = @Name, TotalPoints = @TotalPoints WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(updateCountryQuery, new { c.Name, Id = id, c.TotalPoints });
            if (rowsAffected <= 0)
            {
                return Result.Fail($"No country found with id {id} to update.");
            }

            return Result.Ok().WithSuccess($"Country with id {id} was updated successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"An error occurred while updating the country: {ex.Message}");
        }
    }

    public async Task<Result> DeleteCountry(int id)
    {
        if (id <= 0)
            return Result.Fail("Country Id must be greater than 0.");

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync( "DELETE FROM Teams WHERE CountryId = @CountryId", new { CountryId = id }, transaction);
            
            var rowsAffected = await connection.ExecuteAsync("DELETE FROM Countries WHERE Id = @Id", new { Id = id }, transaction);
            if (rowsAffected <= 0)
            {
                await transaction.RollbackAsync();
                return Result.Fail($"No country found with id {id} to delete.");
            }

            transaction.Commit();
            return Result.Ok().WithSuccess($"Country with id {id} was deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"An error occurred while deleting the country: {ex.Message}");
        }
    }

    public async Task<Result> DeleteCountryByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail("Country name must not be empty or whitespace.");

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            var countryId = await connection.QuerySingleOrDefaultAsync<int?>("SELECT Id FROM Countries WHERE Name = @Name", new { Name = name.Trim() }, transaction);
            if (countryId is null)
            {
                transaction.Rollback();
                return Result.Fail($"No country found with name '{name}' to delete.");
            }

            await connection.ExecuteAsync("DELETE FROM Teams WHERE CountryId = @CountryId", new { CountryId = countryId.Value }, transaction);
            var rowsAffected = await connection.ExecuteAsync( "DELETE FROM Countries WHERE Id = @Id", new { Id = countryId.Value },transaction);

            if (rowsAffected <= 0)
            {
                transaction.Rollback();
                return Result.Fail($"Country with name '{name}' was found, but could not be deleted.");
            }
            
            transaction.Commit();
            return Result.Ok().WithSuccess($"Country with name {name} was deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"An error occurred while deleting the country: {ex.Message}");
        }
    }

    public async Task<Result> DeleteCountries()
    {
        throw new NotImplementedException("This method is not implemented yet. Deleting all countries is not allowed to prevent accidental data loss.");
        //using var connection = new SqliteConnection(_connectionString);

        //var result = await connection.ExecuteAsync("DELETE FROM Teams");
        //var result2 = await connection.ExecuteAsync("DELETE FROM Countries");
        //return result > 0 && result2 > 0;
    }
}
