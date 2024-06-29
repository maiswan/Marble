namespace Maiswan.Marble;

internal static class RandomExtensions
{
    internal static double NextDouble(this Random random, double min, double max)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);

        return random.NextDouble() * (max - min) + min;
    }
}
