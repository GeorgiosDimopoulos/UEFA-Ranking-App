using API.Data.DTOs;
using Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            JsonSerializerOptions JsonOpts = new()
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var teams = await _httpClient.GetFromJsonAsync<TeamResponse[]?>("api/teams", JsonOpts);
            return teams;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching teams: {ex.Message}");
            throw;
        }
    }

    public async Task<TeamResponse[]?> GetTeamsByCompetition(Competition c)
    {
        try
        {
            JsonSerializerOptions JsonOpts = new()
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var teams = await _httpClient.GetFromJsonAsync<TeamResponse[]?>($"api/teams?competition={c}", JsonOpts);
            return teams;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching teams: {ex.Message}");
            throw;
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
