using Core.Models;

namespace UI.Clients;

public class CountriesClient
{
    private readonly HttpClient _httpClient;

    public CountriesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Country[]?> GetCountries()
    {
        return _httpClient.GetFromJsonAsync<Country[]?>("countries");
    }
}
