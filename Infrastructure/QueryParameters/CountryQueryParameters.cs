namespace Infrastructure.QueryParameters;

public class CountryQueryParameters
{
    public bool IncludeTeams { get; set; } = false;
    public bool IncludeMatches { get; set; } = false;
}
