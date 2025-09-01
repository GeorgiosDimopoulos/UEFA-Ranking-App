using Core.Data.Models;

namespace Core.DataAccess
{
    public static class Databasefeeder
    {
        private static Team[]? teams;
        private static Country[]? countries;

        public static Team[] FeedTeams()
        {
            teams =
            [
                new Team
                {
                    Name = "AEK",
                    Competition = Competition.ConferenceLeague,
                    Country = new Country() { Name = "Greece", Id = 1 },
                    IsActive = true,
                    Id = 1,
                    Points = 2

                },
                new Team
                {
                    Name = "Vfb",
                    Competition = Competition.EuropaLeague,
                    Country = new Country() { Name = "Germany", Id = 2 },
                    IsActive = false,
                    Id = 2,
                    Points = 0
                }   ,
                new Team
                {
                    Name = "Sevilla",
                    Competition = Competition.None,
                    Country = new Country() { Name = "Spain", Id = 4 },
                    IsActive = false,
                    Id = 2,
                    Points = 0
                }
            ];

            return teams;
        }

        public static Country[] FeedCountries()
        {
            countries =
            [
                new Country { Id = 1, Name = "Greece", Position = 11 , Teams = [new Team { Points = 40000 }] },
                new Country { Id = 2, Name = "Germany", Position = 3, Teams = [new Team { Points = 55000 }]  },
                new Country { Id = 3, Name = "Cyprus" , Position = 19, Teams = [new Team { Points = 30000 }]  },
            ];

            return countries;
        }
    }
}
