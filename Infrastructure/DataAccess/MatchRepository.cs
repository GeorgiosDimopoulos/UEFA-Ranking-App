
using Dapper;
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

        var sqlQuery = @"SELECT * FROM Matches WHERE CountryId  = @cid";
        var matches = await connection.QueryAsync<Match>(sqlQuery, new { id = cid });
        return matches.ToList();
    }

    public async Task<List<Match>> GetMatchesByTeamId(int tid)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sqlQuery = @"SELECT * FROM Matches WHERE TeamId = @tid";
        var matches = await connection.QueryAsync<Match>(sqlQuery, new { id = tid });
        return matches.ToList();
    }

    public Task<List<Match>> GetMatchesByRound(int r)
    {
        // ToDo: not yet implemented
        return null;
    }

    public async Task<bool> AddMatch(Match m)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var createQuery = @"INSERT INTO Matches (Result, Round, HomeTeamId, Competition, AwayTeamId) VALUES (@Result, @Round, @HomeTeamId, @Competition, @AwayTeamId);SELECT last_insert_rowid()";
        var id = await connection.ExecuteScalarAsync<long>(createQuery, new
        {
            m.Result,
            m.Round,
            m.HomeTeamId,
            m.AwayTeamId
        });

        return id > 0;
    }

    public async Task<bool> UpdateMatch(Match m, int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var updateQuery = @"UPDATE Matches SET Result = @Result, Round = @Round, HomeTeamId = @HomeTeamId, AwayTeamId = @AwayTeamId WHERE Id = @Id";
        var result = await connection.ExecuteAsync(updateQuery, new
        {
            m.Result,
            m.Round,
            m.HomeTeamId,
            m.AwayTeamId,
            Id = id
        });

        // ToDo: update both teams points

        return result > 0;
    }

    public async Task<bool> DeleteMatch(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var deleteQuery = @"DELETE Matches WHERE Id = @Id";
        var result = await connection.ExecuteAsync(deleteQuery, id);

        // ToDo: remove the match from both teams

        return result > 0;
    }
}
