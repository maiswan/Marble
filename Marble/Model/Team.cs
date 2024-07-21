using System.Text.Json.Serialization;

namespace Maiswan.Marble;

public readonly record struct Team
{
    public required int Id { get; init; }

    public int PreviousPopulation { get; init; }

    public required int Population { get; init; }
}