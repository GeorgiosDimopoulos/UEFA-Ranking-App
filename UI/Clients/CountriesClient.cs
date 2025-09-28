using API.Data.DTOs;
using System.Net.Http.Json;

namespace UI.Clients;

public class CountriesClient
{
    private readonly HttpClient _httpClient;

    public CountriesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CountryResponse[]?> GetCountries()
    {
        var countries = await _httpClient.GetFromJsonAsync<CountryResponse[]?>("api/countries");
        return countries;
    }

    public async Task<CountryResponse?> GetCountry(string n)
    {
        var country = await _httpClient.GetFromJsonAsync<CountryResponse?>($"api/countries/{n}");
        return country;
    }

    public async Task<bool?> AddCountry(CountryRequest cr)
    {
        var url = $"api/countries?name={cr.Name}&totalPoints={cr.TotalPoints}";
        var response = await _httpClient.PostAsync(url, null);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"POST /api/countries => {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
        }
        return response.IsSuccessStatusCode;
    }
}
