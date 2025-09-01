using System;
using Core.Data.Models;

namespace Core.DataAccess
{
    public class Databasefeeder
    {
        public Country[] Countries { get; set; }
        public Team[]? Teams { get; set; }

        public Databasefeeder()
        {
            try
            {
                Countries = [
                    new()
                    {
                        Id = 1,
                        Name = "Greece",
                        Position = 11
                    },
                    new()
                    {
                        Id = 2,
                        Name = "Germany",
                        Position = 3
                    },
                    new()
                    {
                        Id = 3,
                        Name = "Spain",
                        Position = 2
                    }];

                Teams = [
                    new Team
                    {
                        Name = "AEK",
                        Competition = Competition.ConferenceLeague,
                        Country = Countries.First(c => c.Name.Equals("Greece")),
                        IsActive = true,
                        Id = 1,
                        Points = 2
                        },
                    new Team
                    {
                        Name = "Vfb",
                        Competition = Competition.EuropaLeague,
                        Country = Countries.First(c => c.Name.Equals("Germany")),
                        IsActive = false,
                        Id = 2,
                        Points = 0
                    },
                    new Team
                    {
                        Name = "Sevilla",
                        Competition = Competition.None,
                        Country = Countries.First(c => c.Name.Equals("Spain")),
                        IsActive = false,
                        Id = 2,
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
}
