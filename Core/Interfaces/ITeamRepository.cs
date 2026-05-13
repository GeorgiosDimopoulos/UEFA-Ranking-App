namespace Core.Interfaces;

public interface ITeamRepository
{
    public Task<List<Team>> GetAllTeams();        
    public Task<Team?> GetTeamById(int id);
    public Task<Team?> GetTeamByName(string name); 
    public Task<List<Team?>> GetTeamsByCountryId(int countryId);    
    
    public Task<bool> AddTeam(Team team, string country);
    public Task<bool> UpdateTeam(Team t, string name);
    public Task<bool> DeleteTeam(int id);
    public Task<bool> DeleteTeamByName(string n);
    public Task<bool> UpdateTeamPoints(string t);
}
