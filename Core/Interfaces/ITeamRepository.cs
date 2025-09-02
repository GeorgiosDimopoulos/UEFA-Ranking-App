using Core.Models;

namespace Core.Interfaces;

public interface ITeamRepository
{
    public Task<List<Team>> GetAllTeams();
    public Task<Team?> GetTeamById(int id);
    public Task<IEnumerable<Team?>> GetTeamsByCountryId(int countryId);
    public Task AddTeam(Team team, string countryName, Competition competition, int position);
    public Task UpdateTeam(Team t);
    public Task DeleteTeam(int id);
}
