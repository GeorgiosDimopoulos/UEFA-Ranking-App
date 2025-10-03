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
        var url = $"api/teams?" +
               $"name={Uri.EscapeDataString(tr.Name)}" +
               $"&countryName={Uri.EscapeDataString(tr.CountryName)}" +
               $"&competition={tr.Competition}" +
               $"&points={tr.Points}" +
               $"&isActive={tr.IsActive}";
        var response = await _httpClient.PostAsync(url, null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool?> UpdateTeam(TeamRequest tr, string originalName)
    {
        var url = $"api/teams/{Uri.EscapeDataString(originalName)}" +
               $"?name={Uri.EscapeDataString(tr.Name)}" +
               $"&countryName={Uri.EscapeDataString(tr.CountryName)}" +
               $"&competition={(int)tr.Competition}" +
               $"&points={tr.Points}" +
               $"&isActive={tr.IsActive}";
        var response = await _httpClient.PutAsync(url, null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool?> DeleteTeam(int id)
    {
        var url = $"api/teams/{id}";
        var response = await _httpClient.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }
}
