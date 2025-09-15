using Core.Models;

namespace UI.Clients;

public class TeamsClient
{
    private readonly HttpClient _httpClient;

    public TeamsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Team[]?> GetTeams()
    {
        return _httpClient.GetFromJsonAsync<Team[]?>("teams");
    }
}
