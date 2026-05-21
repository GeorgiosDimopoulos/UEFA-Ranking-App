namespace Core.Models;

public enum Competition
{
    None = 0,
    ChampionsLeague = 1,
    EuropaLeague = 2,
    ConferenceLeague = 3
}

public enum Round
{
    First = 1,
    Second = 2,
    Third = 3,
    Fourth = 4,
    Fifth = 5,
    Sixth = 6,
    Seventh = 7,
    Eighth = 8,
    PlayOffs,
    RoundOf16,
    Quarter,
    SemiFinals,
    Final,
}