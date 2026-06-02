using Dapper;
using FluentResults;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DataAccess;

public class MatchRepository : IMatchRepository
{
    private readonly string _connectionString;

    public MatchRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string is not set.");
        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException("Connection string is not set.");
    }

    public async Task<List<Match>> GetAllMatches()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var query = @"SELECT * FROM Matches Order By Round DESC";
        var matches = await connection.QueryAsync<Match>(query);

        return matches.ToList();
    }

    public async Task<List<Match>> GetMatchesByCountryId(int cid)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sqlQuery = @"SELECT * FROM Matches WHERE HomeTeamName IN (SELECT Name FROM Teams WHERE CountryId = @cid) OR 
                       AwayTeamName IN (SELECT Name FROM Teams WHERE CountryId = @cid)";
        var matches = await connection.QueryAsync<Match>(sqlQuery, new { cid });
        return matches.ToList();
    }

    public async Task<List<Match>> GetMatchesByTeamName(string n)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sqlQuery = @"SELECT * FROM Matches WHERE HomeTeamName = @n OR AwayTeamName = @n";
        var matches = await connection.QueryAsync<Match>(sqlQuery, new { n });
        return matches.ToList();
    }

    public async Task<List<Match>> GetMatchesByCompetition(Competition c)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sqlQuery = @"SELECT * FROM Matches WHERE Competition = @competition";
        var matches = await connection.QueryAsync<Match>(sqlQuery, new { competition = (int)c });
        return matches.ToList();
    }

    public async Task<List<Match>> GetMatchesByRound(int r)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sqlQuery = @"SELECT * FROM Matches WHERE Round = @r";
        var matches = await connection.QueryAsync<Match>(sqlQuery, new { r });
        return matches.ToList();
    }

    public async Task<Result<Match>> AddMatch(Match m)
    {
        if (m.HomeTeamName == m.AwayTeamName)
            return Result.Fail<Match>("Home team and away team cannot be the same.");

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var homeTeamExists = await connection.QuerySingleOrDefaultAsync<Team>("SELECT * FROM Teams WHERE Name = @Name", new { Name = m.HomeTeamName });
            var awayTeamExists = await connection.QuerySingleOrDefaultAsync<Team>("SELECT * FROM Teams WHERE Name = @Name", new { Name = m.AwayTeamName });
            if (homeTeamExists is null || awayTeamExists is null)
                return Result.Fail<Match>("Both home team and away team must exist in the database.");

            var existingMatchInRound = await connection.QuerySingleOrDefaultAsync<Match>(
                @"SELECT * FROM Matches WHERE Round = @Round AND HomeTeamName = @HomeTeamName AND AwayTeamName = @AwayTeamName",
                new
                {
                    m.Round,
                    m.HomeTeamName,
                    m.AwayTeamName
                });

            if (existingMatchInRound is not null)
                return Result.Fail<Match>($"A match between {m.HomeTeamName} and {m.AwayTeamName} already exists in round {m.Round}.");

            string createQuery =
                @"INSERT INTO Matches (HomeTeamGoals, AwayTeamGoals, Round, HomeTeamName, Competition, AwayTeamName) VALUES (@HomeTeamGoals, @AwayTeamGoals, @Round, @HomeTeamName, @Competition, @AwayTeamName); SELECT last_insert_rowid()";

            var insertMatchesResult = await connection.ExecuteScalarAsync<long>(createQuery, new
            {
                m.Id,
                m.HomeTeamGoals,
                m.AwayTeamGoals,
                m.Round,
                m.HomeTeamName,
                m.AwayTeamName,
                m.Competition
            });

            if (insertMatchesResult <= 0)
                return Result.Fail<Match>($"Unexpected error while inserting match");

            // ToDo: update also teams' countries points
            return Result.Ok(m).WithSuccess($"Match between {m.HomeTeamName} and {m.AwayTeamName} was added successfully with id {m.Id}.");
        }
        catch (Exception ex)
        {
            return Result.Fail<Match>($"Unexpected error while adding match between {m.HomeTeamName} and {m.AwayTeamName}: {ex.Message}");
        }
    }

    public async Task<Result> UpdateMatch(Match m, int id)
    {
        if (id <= 0)
            return Result.Fail($"Invalid match id: {id}");

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var updateQuery = @"UPDATE Matches SET HomeTeamGoals = @HomeTeamGoals, AwayTeamGoals = @AwayTeamGoals, Round = @Round, HomeTeamName = @HomeTeamName, AwayTeamName = @AwayTeamName WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(updateQuery, new
            {
                m.HomeTeamGoals,
                m.AwayTeamGoals,
                m.Round,
                m.HomeTeamName,
                m.AwayTeamName,
                Id = id
            });

            if (rowsAffected > 0)
                return Result.Ok().WithSuccess($"Match with id {id} was updated successfully.");

            return Result.Fail($"Match with id {id} was not found. No row was updated.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Unexpected error while updating match with id {id}: {ex.Message}");
        }
    }

    public async Task<Result> DeleteMatch(int id)
    {
        if (id <= 0)
            return Result.Fail($"Invalid match id: {id}");

        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            const string deleteQuery = """
                DELETE FROM Matches
                WHERE Id = @Id
                """;
            var rowsAffected = await connection.ExecuteAsync(deleteQuery, new { Id = id });

            if (rowsAffected > 0)
                return Result.Ok().WithSuccess("Match was deleted with success");

            return Result.Fail($"Match with id {id} was not found. No row was deleted.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Unexpected error while deleting match with id {id}: {ex.Message}");
        }
    }

    public Task<List<Match>> GetMatchesByTeamId(int id)
    {
        throw new NotImplementedException();
    }
}
