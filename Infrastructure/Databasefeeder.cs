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
                new() { ExternalId = 1, Id = Guid.NewGuid(), Name = "England", Position = 1, TotalPoints = 95000 },
                new() { ExternalId = 2, Id = Guid.NewGuid(), Name = "Spain", Position = 3 , TotalPoints = 79000},
                new() { ExternalId = 3, Id = Guid.NewGuid(), Name = "Germany", Position = 4 , TotalPoints = 75000},
                new() { ExternalId = 4, Id = Guid.NewGuid(), Name = "Italy", Position = 2 , TotalPoints = 84000},
                new() { ExternalId = 5, Id = Guid.NewGuid(), Name = "France", Position = 5 , TotalPoints = 68000},
                new() { ExternalId = 6, Id = Guid.NewGuid(), Name = "Portugal", Position = 7 , TotalPoints = 57000},                                                                                                     
                new() { ExternalId = 7, Id = Guid.NewGuid(), Name = "Netherlands", Position = 6 , TotalPoints = 62000},
                new() { ExternalId = 8, Id = Guid.NewGuid(), Name = "Belgium", Position = 8 , TotalPoints = 55000},
                new() { ExternalId = 9, Id = Guid.NewGuid(), Name = "Turkey", Position = 9 , TotalPoints = 44000},
                new() { ExternalId = 10, Id = Guid.NewGuid(), Name = "Czech", Position = 10 , TotalPoints = 40500},
                new() { ExternalId = 11, Id = Guid.NewGuid(), Name = "Greece", Position = 11 , TotalPoints = 37500}];

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
