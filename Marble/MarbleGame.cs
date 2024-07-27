using System.Collections.Immutable;
using System.Numerics;

namespace Maiswan.Marble;

public class MarbleGame(IEnumerable<TeamBase> teams)
{
    public int DeathIfFewer { get; set; }

    public event EventHandler<MarbleGameChangedEventArgs>? GameStepped;
    public event EventHandler<MarbleGameChangedEventArgs>? GameEnded;

    private readonly List<TeamBase> teams = new(teams);
    public int TotalPopulation => teams.Sum(x => x.Population);
    public IImmutableList<TeamBase> Teams => teams.ToImmutableList();
    public IImmutableList<TeamBase> AliveTeams => teams.Where(x => x.Population != 0).ToImmutableList();

    private int iteration;

    private void RaiseGameChangedEvent(bool isGameOver)
    {
        MarbleGameChangedEventArgs e = new()
        {
            IsGameOver = isGameOver,
            Iteration = iteration,
            Teams = Teams,
        };
        EventHandler<MarbleGameChangedEventArgs>? handler = isGameOver ? GameEnded : GameStepped;
        handler?.Invoke(this, e);
    }

    private void CheckEnd()
    {
        iteration++;
        bool isGameOver = TotalPopulation == 0;
        RaiseGameChangedEvent(isGameOver);
    }

    private static void ForeachTeam<T>(Func<int, T, T, int> formula, T min, T max, IEnumerable<TeamBase> teams) where T : INumber<T>
    {
        foreach (TeamBase team in teams)
        {
            team.Population = formula(team.Population, min, max);
        }
    }

    public void Multiply(double rate, params TeamBase[] teams) => Multiply(rate, rate, teams);
    public void Multiply(double min, double max, params TeamBase[] teams)
    {
        static int MultiplyFormula(int population, double min, double max)
            => (int)(population * Random.Shared.NextDouble(min, max));

        IEnumerable<TeamBase> targets = teams.Length == 0 ? this.teams : teams;
        ForeachTeam(MultiplyFormula, min, max, targets);
        CheckEnd();
    }


    public void Add(int amount, params TeamBase[] teams) => Add(amount, amount, teams);
    public void Add(int min, int max, params TeamBase[] teams)
    {
        static int AddFormula(int population, int min, int max)
            => population + Random.Shared.Next(min, max);

        IEnumerable<TeamBase> targets = teams.Length == 0 ? this.teams : teams;
        ForeachTeam(AddFormula, min, max, targets);
        CheckEnd();
    }

    public void Set(int amount, params TeamBase[] teams) => Set(amount, amount, teams);
    public void Set(int min, int max, params TeamBase[] teams)
    {
        static int SetFormula(int population, int min, int max)
            => Random.Shared.Next(min, max);

        IEnumerable<TeamBase> targets = teams.Length == 0 ? this.teams : teams;
        ForeachTeam(SetFormula, min, max, targets);
        CheckEnd();
    }
}
