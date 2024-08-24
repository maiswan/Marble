namespace Maiswan.Marble;

public class MarbleGameChangedEventArgs : EventArgs
{
    public required int Iteration { get; init; }

    public bool IsGameOver => TotalPopulation == 0;

    public required IReadOnlyList<TeamBase> Teams { get; init; }
    public int TotalPopulation => Teams.Sum(x => x.Population);
    public IReadOnlyList<TeamBase> AliveTeams => Teams.Where(x => x.Population != 0).ToList();
}
