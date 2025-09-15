using Core.Models;

namespace UI.Clients;

public class TeamsClient
{
    private readonly HttpClient _httpClient;

    public TeamsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Team[]?> GetTeams()
    {
        return  await _httpClient.GetFromJsonAsync<Team[]?>("teams");
    }
}
