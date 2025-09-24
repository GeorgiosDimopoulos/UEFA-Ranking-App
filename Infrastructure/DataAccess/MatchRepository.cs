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

    public async Task<List<Match>> GetMatchesByCompetition(int c)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sqlQuery = @"SELECT * FROM Matches WHERE Competition = @c";
        var matches = await connection.QueryAsync<Match>(sqlQuery, new { c });
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

    public async Task<bool> AddMatch(Match m)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var createQuery = @"INSERT INTO Matches (Result, Round, HomeTeamName, Competition, AwayTeamName) VALUES (@Result, @Round, @HomeTeamName, @Competition, @AwayTeamName);SELECT last_insert_rowid()";
        var id = await connection.ExecuteScalarAsync<long>(createQuery, new
        {
            m.Result,
            m.Round,
            m.HomeTeamName,
            m.AwayTeamName,
            m.Competition
        });

        // ToDo: update both teams matches played and points
        int newHomeTeamPoints = 0;
        int newAwayTeamPoints = 0;

        string homeTeamName;
        string awayTeamName;

        if (m.Result == MatchResult.HomeWin)
        {
            newHomeTeamPoints = 3;
            newAwayTeamPoints = 0;
        }
        else if (m.Result == MatchResult.AwayWin)
        {
            newHomeTeamPoints = 0;
            newAwayTeamPoints = 3;
        }
        else
        {
            newHomeTeamPoints = 1;
            newAwayTeamPoints = 1;
        }   
        
        var homeTeamPoints = @"SELECT Points FROM Teams WHERE Name =@homeTeamName";
        var awayTeamPoints = @"SELECT Points FROM Teams WHERE Name = @awayTeamName";

        var finalHomeTeamPoints = newHomeTeamPoints + homeTeamPoints;
        var finalAwayTeamPoints = newAwayTeamPoints + awayTeamPoints;

        return id > 0;
    }

    public async Task<bool> UpdateMatch(Match m, int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var updateQuery = @"UPDATE Matches SET Result = @Result, Round = @Round, HomeTeamName = @HomeTeamName, AwayTeamName = @AwayTeamName WHERE Id = @Id";
        var result = await connection.ExecuteAsync(updateQuery, new
        {
            m.Result,
            m.Round,
            m.HomeTeamName,
            m.AwayTeamName,
            m.Competition,
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
