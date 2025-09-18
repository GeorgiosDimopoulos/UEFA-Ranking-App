using API.Data.DTOs;

namespace UI.Clients;

public class TeamsClient
{
    private readonly HttpClient _httpClient;

    public TeamsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TeamDto[]?> GetTeams()
    {
        return await _httpClient.GetFromJsonAsync<TeamDto[]?>("api/teams");
    }

    public async Task<TeamDto[]?> GetTeamsByCountry(string n)
    {
        return await _httpClient.GetFromJsonAsync<TeamDto[]?>($"api/teams/{n}");
    }

    public async Task<TeamDto?> GetTeam(int id)
    {
        var t = await _httpClient.GetFromJsonAsync<TeamDto?>($"api/teams/{id}");
        return t;
    }
}
