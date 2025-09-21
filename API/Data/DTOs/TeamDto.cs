namespace API.Data.DTOs;

public class TeamResponse
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Points { get; set; }
    public int Position { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public int Competition { get; set; }
}

public class TeamRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Points { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public int Competition { get; set; }
}
