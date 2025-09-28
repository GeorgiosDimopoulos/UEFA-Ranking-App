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

    public async Task<bool> AddMatch(Match m) // ToDo: change it to IResult with more details
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var homeTeamExists = await connection.ExecuteScalarAsync<int>("SELECT * FROM Teams WHERE Name = @Name", new { Name = m.HomeTeamName });
        var awayTeamExists = await connection.ExecuteScalarAsync<int>("SELECT * FROM Teams WHERE Name = @Name", new { Name = m.AwayTeamName });
        if (homeTeamExists <= 0 || awayTeamExists <= 0)
        {
            return false;
        }

        string createQuery;
        if (m.AwayTeamGoals != 99 && m.AwayTeamGoals != 99)        
            createQuery = @"INSERT INTO Matches (HomeTeamGoals, AwayTeamGoals, Round, HomeTeamName, Competition, AwayTeamName) VALUES (@HomeTeamGoals, @AwayTeamGoals, @Round, @HomeTeamName, @Competition, @AwayTeamName);SELECT last_insert_rowid()";        
        else        
            createQuery = @"INSERT INTO Matches (HomeTeamGoals, AwayTeamGoals, Round, HomeTeamName, Competition, AwayTeamName) VALUES (@HomeTeamGoals, @AwayTeamGoals, @Round, @HomeTeamName, @Competition, @AwayTeamName);SELECT last_insert_rowid()";
               
        var insertMatchesResult = await connection.ExecuteScalarAsync<long>(createQuery, new
        {
            m.HomeTeamGoals,
            m.AwayTeamGoals,
            m.Round,
            m.HomeTeamName,
            m.AwayTeamName,
            m.Competition
        });

        if (insertMatchesResult <= 0)
        {
            return false;
        }

        // ToDo: update also teams' countries points

        return true;
    }

    public async Task<bool> UpdateMatch(Match m, int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var updateQuery = @"UPDATE Matches SET Result = @Result, Round = @Round, HomeTeamName = @HomeTeamName, AwayTeamName = @AwayTeamName WHERE Id = @Id";
        var result = await connection.ExecuteAsync(updateQuery, new
        {
            m.HomeTeamGoals,
            m.AwayTeamGoals,
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
