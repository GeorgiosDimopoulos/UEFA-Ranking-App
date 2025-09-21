using API.Data.DTOs;

namespace UI.Clients;

public class TeamsClient
{
    private readonly HttpClient _httpClient;

    public TeamsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TeamResponse[]?> GetTeams()
    {
        return await _httpClient.GetFromJsonAsync<TeamResponse[]?>("api/teams");
    }

    public async Task<TeamResponse[]?> GetTeamsByCountry(string n)
    {
        return await _httpClient.GetFromJsonAsync<TeamResponse[]?>($"api/teams/{n}");
    }

    public async Task<TeamResponse?> GetTeam(int id)
    {
        var t = await _httpClient.GetFromJsonAsync<TeamResponse?>($"api/teams/{id}");
        return t;
    }
}
