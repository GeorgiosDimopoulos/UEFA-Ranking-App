using API.Data.DTOs;

namespace UI.Clients;

public class MatchesClient
{
    private readonly HttpClient _httpClient;

    public MatchesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MatchResponse>?> GetMatches()
    {
        var matches = await _httpClient.GetFromJsonAsync<MatchResponse[]?>("api/matches");
        return matches?.ToList();
    }

    public async Task<MatchResponse> GetMatch(string n)
    {
        var match = await _httpClient.GetFromJsonAsync<MatchResponse?>($"api/match/{n}");
        return match ?? new();
    }
}
