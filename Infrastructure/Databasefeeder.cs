using System;

namespace Infrastructure;

public class DatabaseFeeder
{
    public Country[] Countries { get; set; }
    public Team[]? Teams { get; set; }

    public DatabaseFeeder()
    {
        try
        {
            Countries = [
                new() { ExternalId = 1, Id = Guid.NewGuid(), Name = "England", Position = 1 },
                new() { ExternalId = 2, Id = Guid.NewGuid(), Name = "Spain", Position = 2 },
                new() { ExternalId = 3, Id = Guid.NewGuid(), Name = "Germany", Position = 3 },
                new() { ExternalId = 4, Id = Guid.NewGuid(), Name = "Italy", Position = 4 },
                new() { ExternalId = 5, Id = Guid.NewGuid(), Name = "France", Position = 5 },
                new() { ExternalId = 6, Id = Guid.NewGuid(), Name = "Portugal", Position = 6 },
                new() { ExternalId = 7, Id = Guid.NewGuid(), Name = "Netherlands", Position = 7 },
                new() { ExternalId = 8, Id = Guid.NewGuid(), Name = "Belgium", Position = 8 },
                new() { ExternalId = 9, Id = Guid.NewGuid(), Name = "Turkey", Position = 9 },
                new() { ExternalId = 10, Id = Guid.NewGuid(), Name = "Czech", Position = 10 },
                new() { ExternalId = 11, Id = Guid.NewGuid(), Name = "Greece", Position = 11 }];

            Teams = [
                new Team {
                    Id = Guid.NewGuid(),
                    Name = "AEK",
                    Competition = Competition.ConferenceLeague,
                    Country = Countries.FirstOrDefault(c => c.Name.Equals("Greece")) ?? throw new InvalidOperationException("Country not found"),
                    IsActive = true,
                    ExternalId = 1,
                    Points = 2 },
                new Team {
                    Id = Guid.NewGuid(),
                    Name = "Vfb",
                    Competition = Competition.EuropaLeague,
                    Country = Countries.FirstOrDefault(c => c.Name.Equals("Germany")) ?? throw new InvalidOperationException("Country not found"),
                    IsActive = false,
                    ExternalId = 2,
                    Points = 0 },
                new Team {
                    Id = Guid.NewGuid(),
                    Name = "Sevilla",
                    Competition = Competition.None,
                    Country = Countries.FirstOrDefault(c => c.Name.Equals("Spain")) ?? throw new InvalidOperationException("Country not found"),
                    IsActive = false,
                    ExternalId = 3,
                    Points = 0
                }];

            foreach (var c in Countries)
            {
                c.Teams = Teams.Where(t => t.Country.Name.Equals(c.Name)).ToArray();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Team[]? GetTeams()
    {
        return Teams;
    }

    public Country[]? GetCountries()
    {
        return Countries;
    }
}
