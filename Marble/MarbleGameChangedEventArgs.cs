using System.Collections.Immutable;

namespace Maiswan.Marble;

public class MarbleGameChangedEventArgs : EventArgs
{
    public required bool IsGameOver { get; init; }

    public required int Iteration { get; init; }

    public required IImmutableList<TeamBase> Teams { get; init; }
    public int TotalPopulation => Teams.Sum(x => x.Population);
    public IImmutableList<TeamBase> AliveTeams => Teams.Where(x => x.Population != 0).ToImmutableList();
}
