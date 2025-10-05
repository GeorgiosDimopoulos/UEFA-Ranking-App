using System.Text.Json;
using System.Text.Json.Serialization;
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
        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        var matches = await _httpClient.GetFromJsonAsync<MatchResponse[]?>("api/matches", opts);
        return matches?.ToList();
    }

    public async Task<List<MatchResponse>?> GetMatchesByTeam(string n)
    {
        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        var teamMatches = await _httpClient.GetFromJsonAsync<MatchResponse[]?>($"api/matches/by-team?n={Uri.EscapeDataString(n)}", opts);
        return teamMatches?.ToList();
    }

    public async Task<MatchResponse> GetMatch(string n)
    {
        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        var match = await _httpClient.GetFromJsonAsync<MatchResponse?>($"api/match/{n}", opts);
        return match ?? new();
    }

    public async Task<bool?> SaveMatch(MatchRequest mr)
    {
        var url = $"api/matches?id={mr.Id}" +
            $"&homeTeamName={Uri.EscapeDataString(mr.HomeTeamName)}" +
            $"&awayTeamName={Uri.EscapeDataString(mr.AwayTeamName)}" +
            $"&score={(mr.Score != null ? Uri.EscapeDataString(mr.Score) : string.Empty)}" +
            $"&round={(int)mr.Round}" +
            $"&competition={(int)mr.Competition}";

        var response = await _httpClient.PutAsync(url, null);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"PUT /api/matches => {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
        }

        return response.IsSuccessStatusCode;
    } 

    public async Task<bool?> AddMatch(MatchRequest mr)
    {
        var url = $"api/matches?" +
            $"homeTeamName={Uri.EscapeDataString(mr.HomeTeamName)}" +
            $"&awayTeamName={Uri.EscapeDataString(mr.AwayTeamName)}" +    
            $"&score={(mr.Score != null ? Uri.EscapeDataString(mr.Score) : string.Empty)}" +
            $"&round={(int)mr.Round}" +
            $"&competition={(int)mr.Competition}";

        var response = await _httpClient.PostAsync(url, null);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"POST /api/matches => {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
        }

        return response.IsSuccessStatusCode;
    }
}
