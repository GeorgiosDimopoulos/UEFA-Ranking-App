namespace Core.Interfaces;

public interface IMatchRepository
{
    public Task<List<Match>> GetAllMatches(); 
    public Task<Dictionary<int, List<Match>>> GetMatchesByTeams(int[] id);
    public Task<List<Match>> GetMatchesByTeamId(int id);
    public Task<List<Match>> GetMatchesByCountryId(int id);
    public Task<List<Match>> GetMatchesByRound(int r);
}
