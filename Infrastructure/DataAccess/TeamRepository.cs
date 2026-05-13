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

        var sql = @"SELECT t.Id, t.Name, t.IsActive, t.CountryId, t.Competition, c.Id, c.Name FROM Teams t JOIN Countries c ON t.CountryId = c.Id";
        var teams = await connection.QueryAsync<Team, Country, Team>(sql, (team, country) =>
        {
            team.CountryId = country.Id; return team;
        }, splitOn: "Id");
        return teams.ToList();
    }

    public async Task<Team?> GetTeamById(int id)
    {
        if (id <= 0)
            return null;

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
        if (countryId <= 0)
            return [];

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var countryTeams = await connection.QueryAsync<Team>("SELECT * FROM Teams WHERE CountryId = @CountryId", new { CountryId = countryId });
        if (!countryTeams.Any())
        {
            return [];
        }
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
            Matches = [],
        };

        var insertTeamQuery = "INSERT INTO Teams (Name, IsActive, CountryId, Competition) VALUES (@Name, @IsActive, @CountryId, @Competition)";
        var result = await connection.ExecuteAsync(insertTeamQuery, newTeam);

        return result > 0;
    }

    public async Task<bool> UpdateTeam(Team t, string currentName)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "UPDATE Teams SET Name = @NewName, IsActive = @IsActive, Competition = @Competition WHERE Name = @CurrentName";
        var result = await connection.ExecuteAsync(query, new { CurrentName = currentName, NewName = t.Name, t.IsActive, t.Competition });

        return result > 0;
    }

    public async Task<bool> UpdateTeamPoints(string team)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var teamInfoQuery = "SELECT * FROM Matches WHERE (HomeTeamName = @team OR AwayTeamName = @team) AND HomeTeamGoals IS NOT NULL AND AwayTeamGoals IS NOT NULL";
        var teamMatches = await connection.QueryAsync<Match>(teamInfoQuery, new { team });

        var teamPoints = 0;
        foreach (var teamMatch in teamMatches)
        {
            if (teamMatch.HomeTeamName.Equals(team))
            {
                if (teamMatch.AwayTeamGoals == teamMatch.HomeTeamGoals)
                {
                    teamPoints += 1;
                }
                else if (teamMatch.HomeTeamGoals > teamMatch.AwayTeamGoals)
                {
                    teamPoints += 3;
                }
            }
            else if (teamMatch.AwayTeamName.Equals(team))
            {
                if (teamMatch.AwayTeamGoals == teamMatch.HomeTeamGoals)
                {
                    teamPoints += 1;
                }
                else if (teamMatch.HomeTeamGoals < teamMatch.AwayTeamGoals)
                {
                    teamPoints += 3;
                }
            }
        }

        var updateHomeTeamPointsResult = await connection.ExecuteAsync(@"UPDATE Teams SET Points = @NewPoints WHERE Name = @Name", new { NewPoints = teamPoints, Name = team }, transaction);
        if (updateHomeTeamPointsResult == 0)
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
