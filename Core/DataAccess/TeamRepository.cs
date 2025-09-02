namespace Core.DataAccess;

public class TeamRepository : ITeamRepository
{
    private readonly string _connectionString;

    public TeamRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task AddTeam(Country c)
    {
    }

    public Task DeleteTeam(int id)
    {
    }

    public Task<List<Team>> GetAllTeams()
    {
    }

    public Task<Team?> GetTeamById(int id)
    {
    }

    public Task<Team?> GetTeamsByCountryId(int countryId)
    {
    }

    public Task UpdateTeam(Team t)
    {
    }
}
