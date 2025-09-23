
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

    public async Task<Dictionary<int, List<Match>>> GetMatchesByTeams(int[] id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // ToDo: fix the query
        var query = @"SELECT * FROM Matches GROUP BY TeamId";
        var matches = await connection.QueryAsync<Match>(query);
        
        // return matches.ToDictionary(m => m.HomeTeamId, m=>m);
        return null;
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
}
