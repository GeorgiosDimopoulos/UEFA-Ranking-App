using Microsoft.Data.Sqlite;

namespace Infrastructure.Helpers;

public class DatabaseInitializer
{
    public void EnsureCountryTableExists(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();

        var createCountriesTableQuery = @"CREATE TABLE IF NOT EXISTS Countries(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Position INTEGER NOT NULL,
                NumberOfInitialTeams INTEGER NOT NULL DEFAULT 0,
                NumberOfActiveTeams INTEGER NOT NULL DEFAULT 0,
                TotalPoints REAL)";
        command.CommandText = createCountriesTableQuery;
        var result = command.ExecuteNonQuery();

        var createIndexQuery = "CREATE UNIQUE INDEX IF NOT EXISTS IX_Countries_Name ON Countries(Name)";
        command.CommandText = createIndexQuery;
        command.ExecuteNonQuery();

        var createTeamsTableQuery = @"CREATE TABLE IF NOT EXISTS Teams(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                IsActive INTEGER NOT NULL,
                Competition INTEGER NOT NULL,
                CountryId INTEGER NOT NULL,
                Position INTEGER NOT NULL,
                Points INTEGER NOT NULL,
                FOREIGN KEY (CountryId) REFERENCES Countries(Id) ON DELETE CASCADE)";
        command.CommandText = createTeamsTableQuery;
        command.ExecuteNonQuery();

        var createMatchesTableQuery = @"CREATE TABLE IF NOT EXISTS Matches(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                HomeTeam TEXT NOT NULL,
                AwayTeam TEXT NOT NULL,
                Result INTEGER NOT NULL,
                MatchDate TEXT NOT NULL,
                Competition INTEGER NOT NULL)";
        command.CommandText = createMatchesTableQuery;
        command.ExecuteNonQuery();
    }
}
