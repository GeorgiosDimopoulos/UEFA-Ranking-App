using Core.QueryParameters;

namespace Core.Interfaces;

public interface ITeamRepository
{
    public Task<List<Team>> GetAllTeams(TeamQueryParameters parameters);
    public Task<Team?> GetTeamById(int id, TeamQueryParameters parameters);
    public Task<Team?> GetTeamByName(string name, TeamQueryParameters parameters);
    public Task<List<Team?>> GetTeamsByCountryId(int countryId, TeamQueryParameters queryParameters);

    public Task<bool> AddTeam(Team team, string country);
    public Task<bool> UpdateTeam(Team t, string name);
    public Task<bool> DeleteTeam(int id);
    public Task<bool> DeleteTeamByName(string n);
    public Task<bool> UpdateTeamPoints(string t);
}
