using System.Text.Json.Serialization;

namespace Maiswan.Marble.Demo;

public readonly record struct DemoTeam
{
    public required string Name { get; init; }

    [JsonConverter(typeof(ConsoleColorConverter))]
    public required ConsoleColor Color { get; init; }

    public int PreviousPopulation { get; init; }

    public required int Population { get; init; }
}