using Dapper;
using Microsoft.Data.Sqlite;

namespace Infrastructure.Helpers;

public class DatabaseFeeder
{
    private readonly ITeamRepository teamRepository;
    private readonly ICountryRepository countryRepository;

    public DatabaseFeeder(ICountryRepository countryRepository, ITeamRepository teamRepository)
    {
        this.countryRepository = countryRepository;
        this.teamRepository = teamRepository;
    }

    public bool EnsureRecordsExist(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var countries = connection.ExecuteScalar<int>("SELECT * FROM Countries");
        var teams = connection.ExecuteScalar<int>("SELECT * FROM Teams");

        return countries > 1 && teams > 1;
    }

    public async Task SeedCountriesAndTeams()
    {
        try
        {
            var countries = new Country[]
            {
                new() { Id = 1, Name = "England", TotalPoints = 95000},
                new() { Id = 2, Name = "Spain" , TotalPoints = 79000},
                new() { Id = 3, Name = "Germany" , TotalPoints = 75000},
                new() { Id = 4, Name = "Italy" , TotalPoints = 84000},
                new() { Id = 5, Name = "France" , TotalPoints = 68000},
                new() { Id = 6, Name = "Portugal" , TotalPoints = 57000},
                new() { Id = 7, Name = "Netherlands" , TotalPoints = 62000},
                new() { Id = 8, Name = "Belgium" , TotalPoints = 55000},
                new() { Id =9, Name = "Turkey" , TotalPoints = 44000},
                new() { Id = 10, Name = "Czech" , TotalPoints = 40500},
                new() { Id = 11, Name = "Greece" , TotalPoints = 37500}
            };

            foreach (var c in countries)
            {
                await countryRepository.AddCountry(c);
            }

            var team = new Team
            {
                Id = 1,
                Name = "AEK",
                Competition = Competition.ConferenceLeague,
                CountryId = (countries.FirstOrDefault(c => c.Name.Equals("Greece")) ?? throw new InvalidOperationException("Country not found")).Id,
                IsActive = true,
                Points = 2
            };
            await teamRepository.AddTeam(team, "Greece");

            team = new Team
            {
                Id = 2,
                Name = "Vfb",
                Competition = Competition.EuropaLeague,
                CountryId = (countries.FirstOrDefault(c => c.Name.Equals("Germany")) ?? throw new InvalidOperationException("Country not found")).Id,
                IsActive = false,
                Points = 0
            };
            await teamRepository.AddTeam(team, "Germany");

            team = new Team
            {
                Id = 3,
                Name = "Sevilla",
                Competition = Competition.None,
                CountryId = (countries.FirstOrDefault(c => c.Name.Equals("Spain")) ?? throw new InvalidOperationException("Country not found")).Id,
                IsActive = false,
                Points = 0
            };
            await teamRepository.AddTeam(team, "Spain");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
