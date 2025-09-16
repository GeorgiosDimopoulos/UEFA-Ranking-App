using Microsoft.Data.Sqlite;

namespace Infrastructure.Helpers;

public class DatabaseInitializer
{
    public void EnsureCountryTableExists(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var createCountriesTableQuery = @"CREATE TABLE IF NOT EXISTS Countries(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Position INTEGER NOT NULL,
                NumberOfTeams INTEGER NOT NULL,
                TotalPoints REAL)";

        var createTeamsTableQuery = @"CREATE TABLE IF NOT EXISTS Teams(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                IsActive INTEGER NOT NULL,
                Competition INTEGER NOT NULL,
                CountryId INTEGER NOT NULL,
                Position INTEGER NOT NULL,
                Points INTEGER NOT NULL,
                FOREIGN KEY (CountryId) REFERENCES Countries(Id))";

        ExecuteNonQuery(connection, createCountriesTableQuery);
        ExecuteNonQuery(connection, createTeamsTableQuery);
    }

    private bool ExecuteNonQuery(SqliteConnection connection, string createCountriesTableQuery)
    {
        using var command = connection.CreateCommand();

        command.CommandText = createCountriesTableQuery;
        var result = command.ExecuteNonQuery();
        
        if (result != 0)
        {
            return true;
        }
        return false;
    }
}
