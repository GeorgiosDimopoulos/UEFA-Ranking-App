namespace Core.QueryParameters;

public class TeamQueryParameters
{
    public bool IncludeMatches { get; set; } = true;
    public bool IncludeCountry { get; set; } = true;
    public Competition? Competition { get; set; }
}
