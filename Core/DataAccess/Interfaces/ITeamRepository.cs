namespace Core.DataAccess.Interfaces;

public interface ITeamRepository
{
    public Task<List<Team>> GetAllTeams();
    public Task<Team?> GetTeamById(int id);
    public Task<Team?> GetTeamsByCountryId(int countryId);
    public Task AddTeam(Country c);
    public Task UpdateTeam(Team t);
    public Task DeleteTeam(int id);
}
