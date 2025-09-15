using API.Data.DTOs;

namespace UI.Clients;

public class CountriesClient
{
    private readonly HttpClient _httpClient;

    public CountriesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CountryDto[]?> GetCountries()
    {
        var countries = await _httpClient.GetFromJsonAsync<CountryDto[]?>("api/countries");
        return countries;
    }
}
