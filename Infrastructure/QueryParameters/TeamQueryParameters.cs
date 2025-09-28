namespace Infrastructure.QueryParameters;

public class TeamQueryParameters
{
    public bool IncludeMatches { get; set; } = false;
    //public bool IncludeCountry { get; set; } = false;
    public Competition? Competition { get; set; }
}
