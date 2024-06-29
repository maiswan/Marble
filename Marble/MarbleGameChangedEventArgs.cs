namespace Maiswan.Marble;

internal class MarbleGameChangedEventArgs : EventArgs
{
    public bool IsGameOver { get; init; }

    public int Iteration { get; init; }

    public IReadOnlyCollection<Team> Teams { get; init; }

    public int TotalPopulation => Teams.Sum(x => x.Population);

    public MarbleGameChangedEventArgs(bool isGameOver, int iteration, IReadOnlyCollection<Team> teams)
    {
        IsGameOver = isGameOver;
        Iteration = iteration;
        Teams = teams;
    }
}
