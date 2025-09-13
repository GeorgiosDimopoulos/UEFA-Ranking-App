using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using System;

namespace Infrastructure;

public class DatabaseFeeder
{
    public Country[] Countries { get; set; }
    public Team[]? Teams { get; set; }

    private ICountryRepository teamRepository;
    private ICountryRepository countryRepository;
    private IConfiguration configuration;

    public DatabaseFeeder(ICountryRepository countries, ICountryRepository teams)
    {
        countryRepository = countries;
        teamRepository = teams;
    }

    public void SeedCountriesAndTeams()
    {
        try
        {
            Countries = [
                new() { ExternalId = 1, Id = 1, Name = "England", Position = 1, TotalPoints = 95000 },
                new() { ExternalId = 2, Id = 2, Name = "Spain", Position = 3 , TotalPoints = 79000},
                new() { ExternalId = 3, Id = 3, Name = "Germany", Position = 4 , TotalPoints = 75000},
                new() { ExternalId = 4, Id = 4, Name = "Italy", Position = 2 , TotalPoints = 84000},
                new() { ExternalId = 5, Id = 5, Name = "France", Position = 5 , TotalPoints = 68000},
                new() { ExternalId = 6, Id = 6, Name = "Portugal", Position = 7 , TotalPoints = 57000},                                                                                                     
                new() { ExternalId = 7, Id = 7, Name = "Netherlands", Position = 6 , TotalPoints = 62000},
                new() { ExternalId = 8, Id = 8, Name = "Belgium", Position = 8 , TotalPoints = 55000},
                new() { ExternalId = 9, Id =9, Name = "Turkey", Position = 9 , TotalPoints = 44000},
                new() { ExternalId = 10, Id = 10, Name = "Czech", Position = 10 , TotalPoints = 40500},
                new() { ExternalId = 11, Id = 11, Name = "Greece", Position = 11 , TotalPoints = 37500}];

            Teams = [
                new Team {
                    Id =1,
                    Name = "AEK",
                    Competition = Competition.ConferenceLeague,
                    Country = Countries.FirstOrDefault(c => c.Name.Equals("Greece")) ?? throw new InvalidOperationException("Country not found"),
                    IsActive = true,
                    ExternalId = 1,
                    Points = 2 },
                new Team {
                    Id = 2,
                    Name = "Vfb",
                    Competition = Competition.EuropaLeague,
                    Country = Countries.FirstOrDefault(c => c.Name.Equals("Germany")) ?? throw new InvalidOperationException("Country not found"),
                    IsActive = false,
                    ExternalId = 2,
                    Points = 0 },
                new Team {
                    Id = 3,
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
