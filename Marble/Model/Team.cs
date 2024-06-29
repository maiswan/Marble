using System.Text.Json.Serialization;

namespace Maiswan.Marble;

public readonly record struct Team
{
    public string Name { get; init; } = "";

    [JsonConverter(typeof(ConsoleColorConverter))]
    public ConsoleColor Color { get; init; }

    public int PreviousPopulation { get; init; }

    public int Population { get; init;  }

    public Team()
    { }

    // public TeamStatus(Team team)
    // {
    //     Name = team.Name;
    //     Color = team.Color;
    //     PreviousPopulation = team.PreviousPopulation;
    //     Population = team.Population;
    // }
}