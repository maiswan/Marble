namespace Maiswan.Marble;

public class MarbleGameChangedEventArgs : EventArgs
{
    public required bool IsGameOver { get; init; }

    public required int Iteration { get; init; }

    public required IReadOnlyCollection<Team> Teams { get; init; }

    public int TotalPopulation => Teams.Sum(x => x.Population);
}
