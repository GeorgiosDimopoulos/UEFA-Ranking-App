namespace Core.Interfaces;

public interface IMatchRepository
{
    public List<Match> GetMatchesByTeamId(int id);
}
