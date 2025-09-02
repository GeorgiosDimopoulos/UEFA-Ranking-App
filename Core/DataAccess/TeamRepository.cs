using Core.Data.Models;
using Core.DataAccess.Interfaces;

namespace Core.DataAccess;

public class TeamRepository : ITeamRepository
{
    public Task AddTeam(Country c)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTeam(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Team>> GetAllTeams()
    {
        throw new NotImplementedException();
    }

    public Task<Team?> GetTeamById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Team?> GetTeamsByCountryId(int countryId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateTeam(Team t)
    {
        throw new NotImplementedException();
    }
}
