namespace Core.Interfaces;

public interface IMatchRepository
{
    public Task<List<Match>> GetAllMatches(); 
    public Task<List<Match>> GetMatchesByTeamId(int id);
    public Task<List<Match>> GetMatchesByCountryId(int id);
    public Task<List<Match>> GetMatchesByRound(int r);

    public Task<bool> AddMatch(Match match);
    public Task<bool> UpdateMatch(Match t, int id);
    public Task<bool> DeleteMatch(int id);
}
