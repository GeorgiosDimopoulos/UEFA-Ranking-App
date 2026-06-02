using FluentResults;

namespace Core.Interfaces;

public interface IMatchRepository
{
    public Task<List<Match>> GetAllMatches(); 
    public Task<List<Match>> GetMatchesByTeamName(string n);
    public Task<List<Match>> GetMatchesByTeamId(int id);
    public Task<List<Match>> GetMatchesByCountryId(int id);
    public Task<List<Match>> GetMatchesByRound(int r);

    public Task<List<Match>> GetMatchesByCompetition(Competition c); 

    public Task<Result<Match>> AddMatch(Match match);
    public Task<Result> UpdateMatch(Match t, int id);
    public Task<Result> DeleteMatch(int id);
}
