namespace Infrastructure;

public class Databasefeeder
{
    public Country[] Countries { get; set; }
    public Team[]? Teams { get; set; }

    public Databasefeeder()
    {
        try
        {
            Countries = [
                new() { Id = 1, Name = "England", Position = 1 },
                new() { Id = 2, Name = "Spain", Position = 2 },
                new() { Id = 3, Name = "Germany", Position = 3 },
                new() { Id = 4, Name = "Italy", Position = 4 },
                new() { Id = 5, Name = "France", Position = 5 },
                new() { Id = 6, Name = "Portugal", Position = 6 },
                new() { Id = 7, Name = "Netherlands", Position = 7 },
                new() { Id = 8, Name = "Belgium", Position = 8 },
                new() { Id = 9, Name = "Turkey", Position = 9 },
                new() { Id = 10, Name = "Czech", Position = 10 },
                new() { Id = 11, Name = "Greece", Position = 11 }];

            Teams = [
                new Team {
                    Name = "AEK",
                    Competition = Competition.ConferenceLeague,
                    Country = Countries.FirstOrDefault(c => c.Name.Equals("Greece")) ?? throw new InvalidOperationException("Country not found"),
                    IsActive = true,
                    Id = 1,
                    Points = 2 },
                new Team {
                    Name = "Vfb",
                    Competition = Competition.EuropaLeague,
                    Country = Countries.FirstOrDefault(c => c.Name.Equals("Germany")) ?? throw new InvalidOperationException("Country not found"),
                    IsActive = false,
                    Id = 2,
                    Points = 0 },
                new Team {
                    Name = "Sevilla",
                    Competition = Competition.None,
                    Country = Countries.FirstOrDefault(c => c.Name.Equals("Spain")) ?? throw new InvalidOperationException("Country not found"),
                    IsActive = false,
                    Id = 3,
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
