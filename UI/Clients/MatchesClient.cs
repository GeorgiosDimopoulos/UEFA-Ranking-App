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
        var matches = await _httpClient.GetFromJsonAsync<MatchResponse[]?>("api/Matches");
        return matches?.ToList();
    }

    public async Task<MatchResponse> GetMatch(string n)
    {
        var match = await _httpClient.GetFromJsonAsync<MatchResponse?>($"api/match/{n}");
        return match ?? new();
    }

    public async Task<bool?> AddMatch(MatchRequest mr)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/matches/", mr);
        return response.IsSuccessStatusCode;
    }
}
