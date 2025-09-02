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

        var teams = await connection.QueryAsync<Team>("SELECT * FROM Teams Order By Name ASC");
        return teams.ToList();
    }

    public async Task<Team?> GetTeamById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        var team = await connection.QuerySingleOrDefaultAsync<Team>("SELECT * FROM Teams WHERE Id = @Id", new { Id = id });
        return team;
    }

    public async Task<IEnumerable<Team?>> GetTeamsByCountryId(int countryId)
    {
        using var connection = new SqliteConnection(_connectionString);

        var countryTeams = await connection.QueryAsync<Team>("SELECT * FROM Teams WHERE CountryId = @CountryId", new { CountryId = countryId });
        if (!countryTeams.Any())
            throw new InvalidOperationException("No teams found for the given country ID.");
        return countryTeams;
    }

    public async Task<bool> AddTeam(Team team, string countryName, Competition competition, int position)
    {
        using var connection = new SqliteConnection(_connectionString);

        var availableCountry = await connection.QuerySingleOrDefaultAsync<Country>("SELECT * FROM Countries WHERE Name = @Name", new { Name = countryName }) 
            ?? throw new InvalidOperationException("Country does not exist in the database.");

        var newTeam = new Team
        {
            Name = team.Name,
            Country = availableCountry,
            CountryId = availableCountry.Id,
            Competition = competition,
            Points = 0,
            Position = position,
            Matches = [],
        };

        if (competition != Competition.None)
        {
            newTeam.IsActive = true;
        }

        var insertTeamQuery = "INSERT INTO Teams (Name, IsActive, Points, Position, CountryId, Competition) VALUES (@Name, @IsActive, @Points, @Position, @CountryId, @Competition)";
        var result = await connection.ExecuteAsync(insertTeamQuery, newTeam);

        return result > 0;
    }

    public async Task<bool> UpdateTeam(Team t)
    {
        using var connection = new SqliteConnection(_connectionString);

        var team = await connection.QuerySingleOrDefaultAsync<Team>("SELECT * FROM Teams WHERE Id = @Id", new { Id = t.Id });
        if (team is null)
            throw new InvalidOperationException("Team does not exist in the database.");

        team.Name = t.Name;
        team.IsActive = t.IsActive;
        team.Points = t.Points;
        team.Position = t.Position;

        var updateQuery = "UPDATE Teams SET Name = @Name, IsActive = @IsActive, Points = @Points, Position = @Position WHERE Id = @Id";
        var result = await connection.ExecuteAsync(updateQuery, team);

        return result > 0;
    }

    public async Task<bool> DeleteTeam(int id)
    {        
        using var connection = new SqliteConnection(_connectionString);

        var deleteQuery = "DELETE FROM Teams WHERE Id = @Id";
        var result = await connection.ExecuteAsync(deleteQuery, new { Id = id });

        return result > 0;
    }
}
