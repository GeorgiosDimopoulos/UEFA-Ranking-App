using Dapper;
using Microsoft.Data.Sqlite;

namespace Core.DataAccess;

public class TeamRepository : ITeamRepository
{
    private readonly string _connectionString;

    public TeamRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<Team>> GetAllTeams()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Connection string is not set.");

        using var connection = new SqliteConnection(_connectionString);

        var teams = await connection.QueryAsync<Team>("SELECT * FROM Teams Order By Name ASC");
        return teams.ToList();
    }

    public async Task<Team?> GetTeamById(int id)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Connection string is not set.");
        using var connection = new SqliteConnection(_connectionString);

        var team = await connection.QuerySingleOrDefaultAsync<Team>("SELECT * FROM Teams WHERE Id = @Id", new { Id = id });
        return team;
    }

    public async Task<IEnumerable<Team?>> GetTeamsByCountryId(int countryId)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Connection string is not set.");
        using var connection = new SqliteConnection(_connectionString);

        var countryTeams = await connection.QueryAsync<Team>("SELECT * FROM Teams WHERE CountryId = @Id", new { CountryId = countryId });
        if (countryTeams == null)
            throw new InvalidOperationException("No teams found for the given country ID.");
        return countryTeams;
    }

    public async Task AddTeam(Team team, string countryName, Competition competition, int position)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Connection string is not set.");
        using var connection = new SqliteConnection(_connectionString);

        var availableCountry = await connection.QuerySingleAsync<Country>("SELECT * FROM Countries WHERE Name = @Name", new { Name = countryName });

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

        var insertTeamQuery = "INSERT INTO Teams (Name, IsActive, Points, Position, CountryId, Competition) VALUES (@Name, @IsActive, @Points, @Position, @CountryId, @Competition);";
        await connection.ExecuteAsync(insertTeamQuery, newTeam);
    }

    public async Task UpdateTeam(Team t)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Connection string is not set.");
        using var connection = new SqliteConnection(_connectionString);

        var country = await connection.QuerySingleOrDefaultAsync<Team>("SELECT * FROM Teams WHERE Id = @Id", new { t.Id });
        if (country is null)
            throw new InvalidOperationException("Team does not exist in the database.");

        country.Name = t.Name;
        country.IsActive = t.IsActive;
        country.Points = t.Points;
        country.Position = t.Position;

        var updateQuery = "UPDATE Teams SET Name = @Name, IsActive = @IsActive, Points = @Points, Position = @Position WHERE Id = @Id";
        await connection.ExecuteAsync(updateQuery, country);
    }

    public async Task DeleteTeam(int id)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Connection string is not set.");
        using var connection = new SqliteConnection(_connectionString);

        var team = await connection.QuerySingleOrDefaultAsync<Team>("SELECT * FROM Teams WHERE Id = @Id", new { Id = id }) ?? throw new InvalidOperationException("Team does not exist in the database.");
        var deleteQuery = "DELETE FROM Teams WHERE Id = @Id";
        await connection.ExecuteAsync(deleteQuery, new { Id = id });
    }
}
