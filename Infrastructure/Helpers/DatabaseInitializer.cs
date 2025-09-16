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
                NumberOfTeams INTEGER NOT NULL,
                TotalPoints REAL)";
        command.CommandText = createCountriesTableQuery;
        var result = command.ExecuteNonQuery();

        var createTeamsTableQuery = @"CREATE TABLE IF NOT EXISTS Teams(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                IsActive INTEGER NOT NULL,
                Competition INTEGER NOT NULL,
                CountryId INTEGER NOT NULL,
                Position INTEGER NOT NULL,
                Points INTEGER NOT NULL,
                FOREIGN KEY (CountryId) REFERENCES Countries(Id))";
        command.CommandText = createTeamsTableQuery;
        var result2 = command.ExecuteNonQuery();
    }
}
