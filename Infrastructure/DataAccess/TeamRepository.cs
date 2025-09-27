using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DataAccess;

public class TeamRepository : ITeamRepository
{
    private readonly string _connectionString;

    public TeamRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string is not set.");
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Connection string is not set.");
    }

    public async Task<List<Team>> GetAllTeams()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"SELECT t.Id, t.Name, t.IsActive, t.Points, t.CountryId, t.Competition, c.Id, c.Name FROM Teams t JOIN Countries c ON t.CountryId = c.Id";
        var teams = await connection.QueryAsync<Team, Country, Team>(sql, (team, country) => 
        { 
            team.CountryId = country.Id; return team; 
        }, splitOn: "Id");
        return teams.ToList();
    }

    public async Task<Dictionary<string ,int>> GetTeamsNamesAndPoints() 
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"SELECT Name, Points FROM Teams";
        var teams = await connection.QueryAsync<(string Name, int Points)>(sql);

        return teams.ToDictionary(t => t.Name, t => t.Points);
    }

    public async Task<Team?> GetTeamById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var team = await connection.QuerySingleOrDefaultAsync<Team>("SELECT * FROM Teams WHERE Id = @Id", new { Id = id });
        return team;
    }

    public async Task<Team?> GetTeamByName(string name)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var team = await connection.QuerySingleOrDefaultAsync<Team>("SELECT * FROM Teams WHERE Name = @Name", new { Name = name });
        return team;
    }

    public async Task<List<Team?>> GetTeamsByCountryId(int countryId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var countryTeams = await connection.QueryAsync<Team>("SELECT * FROM Teams WHERE CountryId = @CountryId", new { CountryId = countryId });
        if (!countryTeams.Any())
            throw new InvalidOperationException("No teams found for the given country ID.");
        return countryTeams.ToList()!;
    }

    public async Task<bool> AddTeam(Team team, string country)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var teamExists = await connection.QuerySingleOrDefaultAsync<Team>("SELECT * FROM Teams WHERE Name = @Name", new { team.Name });
        if (teamExists != null)
        {
            return false;
        }

        var availableCountry = await connection.QuerySingleOrDefaultAsync<Country>("SELECT * FROM Countries WHERE Name = @Name", new { Name = country })
            ?? throw new InvalidOperationException("Country does not exist in the database.");

        var newTeam = new Team
        {
            Name = team.Name,
            CountryId = availableCountry.Id,
            Competition = team.Competition,
            IsActive = team.IsActive,
            Points = team.Points,
            Matches = [],
        };

        var insertTeamQuery = "INSERT INTO Teams (Name, IsActive, Points, CountryId, Competition) VALUES (@Name, @IsActive, @Points, @CountryId, @Competition)";
        var result = await connection.ExecuteAsync(insertTeamQuery, newTeam);

        return result > 0;
    }

    public async Task<bool> UpdateTeam(Team t, int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var updateQuery = "UPDATE Teams SET Name = @Name, IsActive = @IsActive, Points = @Points WHERE Id = @Id";
        var result = await connection.ExecuteAsync(updateQuery, new { t.Name, t.IsActive, t.Points, Id = id, t.Competition });

        return result > 0;
    }

    public async Task<bool> UpdateTeamPoints(Match m)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var homeTeamPoints = m.AwayTeamGoals < m.HomeTeamGoals ? 3 : m.AwayTeamGoals == m.HomeTeamGoals ? 1 : 0;
        var awayTeamPoints = m.AwayTeamGoals > m.HomeTeamGoals ? 3 : m.AwayTeamGoals == m.HomeTeamGoals ? 1 : 0;

        using var transaction = connection.BeginTransaction();

        var updateHomeTeamPointsResult = await connection.ExecuteAsync(@"UPDATE Teams SET Points = Points + @NewPoints WHERE Name = @Name", new { NewPoints = homeTeamPoints, Name = m.HomeTeamName });
        var updateAwayTeamPointsResult = await connection.ExecuteAsync(@"UPDATE Teams SET Points = Points + @NewPoints WHERE Name = @Name", new { NewPoints = awayTeamPoints, Name = m.AwayTeamName });
        if (updateHomeTeamPointsResult == 0 || updateAwayTeamPointsResult == 0)
        {
            await transaction.RollbackAsync();
            return false;
        }            

        await transaction.CommitAsync();
        return true;
    }

    public async Task<bool> DeleteTeam(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var deleteQuery = "DELETE FROM Teams WHERE Id = @Id";
        var result = await connection.ExecuteAsync(deleteQuery, new { Id = id });

        return result > 0;
    }

    public async Task<bool> DeleteTeamByName(string Name)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var deleteQuery = "DELETE FROM Teams WHERE Name = @Name";
        var result = await connection.ExecuteAsync(deleteQuery, new { Name });

        return result > 0;
    }
}
