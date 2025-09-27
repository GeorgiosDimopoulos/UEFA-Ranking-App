using API.Data.DTOs;
using Infrastructure.QueryParameters;

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

        try
        {
            var teams = await _httpClient.GetFromJsonAsync<TeamResponse[]?>("api/teams");
            return teams;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching teams: {ex.Message}");
            throw new Exception();
        }
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

    public async Task<bool?> AddTeam(TeamRequest tr)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/teams/", tr);
        return response.IsSuccessStatusCode;
    }
}
