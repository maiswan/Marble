namespace Maiswan.Marble;

internal static class RandomExtensions
{
    internal static double NextDouble(this Random random, double min, double max)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);

        return random.NextDouble() * (max - min) + min;
    }

    internal static IEnumerable<double> NextDoubles(this Random random, double mean, double stdev, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(stdev, 0);

        double[] rates = new double[count];
        for (int i = 0; i < count; i++)
        {
            rates[i] = random.NextDouble();
        }
        double originalMean = rates.Average();
        double originalStdev = Math.Sqrt(rates.Average(x => Math.Pow(x - originalMean, 2)));

        return rates.Select(x => stdev * (x - originalMean) / originalStdev + mean);
    }

    internal static IEnumerable<int> NextInts(this Random random, double mean, double stdev, int count)
    {
        IEnumerable<double> rates = random.NextDoubles(mean, stdev, count);
        return rates.Select(x => (int)x);
    }
}
