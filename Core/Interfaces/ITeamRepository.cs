using Core.QueryParameters;
using FluentResults;

namespace Core.Interfaces;

public interface ITeamRepository
{
    public Task<List<Team>> GetAllTeams(TeamQueryParameters parameters);
    public Task<Team?> GetTeamById(int id, TeamQueryParameters parameters);
    public Task<Team?> GetTeamByName(string name, TeamQueryParameters parameters);
    public Task<List<Team?>> GetTeamsByCountryId(int countryId, TeamQueryParameters queryParameters);

    public Task<Result<Team>> AddTeam(Team team, string country);
    public Task<Result> UpdateTeam(Team t, string name);
    public Task<Result> DeleteTeam(int id);
    public Task<Result> DeleteTeamByName(string n);
    public Task<Result> UpdateTeamPoints(string t);
}
