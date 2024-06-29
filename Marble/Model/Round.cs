namespace Maiswan.Marble;

public readonly record struct Round
{
    public double MultiplierMin { get; init; }

    public double MultiplierMax { get; init; }

    public int Count { get; init; } = 1;

    public Round()
    { }
}