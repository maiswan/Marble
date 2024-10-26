using System.Text.Json.Serialization;

namespace Maiswan.Marble.Demo;

public class DemoTeam : TeamBase
{
    public required string Name { get; init; }

    [JsonConverter(typeof(ConsoleColorConverter))]
    public required ConsoleColor Color { get; init; }
}