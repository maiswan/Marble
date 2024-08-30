using System.Numerics;

namespace Maiswan.Marble;

public class MarbleGame(IEnumerable<TeamBase> teams)
{
    public int DeathIfFewer { get; set; }

    public event EventHandler<MarbleGameChangedEventArgs>? GameStepped;
    public event EventHandler<MarbleGameChangedEventArgs>? GameEnded;

    private readonly List<TeamBase> teams = new(teams);

    public int TotalPopulation => teams.Sum(x => x.Population);
    public bool IsGameOver => TotalPopulation == 0;

    public IReadOnlyList<TeamBase> Teams => teams[..];
    public IReadOnlyList<TeamBase> AliveTeams => teams.Where(x => x.Population != 0).ToList();

    public int Iteration { get; private set; }

    #region Auxiliary

    private void Stepped()
    {
        Iteration++;

        ZeroDyingTeams();
        RaiseEvent();
    }

    private void ZeroDyingTeams()
    {
        foreach (TeamBase team in teams)
        {
            if (team.Population < DeathIfFewer) { team.Population = 0; }
        }
    }

    private void RaiseEvent()
    {
        MarbleGameChangedEventArgs e = new()
        {
            Iteration = Iteration,
            Teams = Teams,
        };
        GameStepped?.Invoke(this, e);

        if (!IsGameOver) { return; }
        GameEnded?.Invoke(this, e);
    }

    private void ForeachTeam<T>(Func<int, T, int> formula, T parameter, IEnumerable<TeamBase>? targets) where T : INumber<T>
    {
        targets ??= AliveTeams;

        foreach (TeamBase target in targets)
        {
            int population = formula(target.Population, parameter);
            target.Population = population >= DeathIfFewer ? population : 0;
        }

        Stepped();
    }

    private void ForeachTeam<T>(Func<int, T, T, int> formula, T parameter1, T parameter2, IEnumerable<TeamBase>? targets) where T : INumber<T>
    {
        targets ??= AliveTeams;

        foreach (TeamBase target in targets)
        {
            target.Population = formula(target.Population, parameter1, parameter2);
        }

        Stepped();
    }

	private void ForeachTeam<T>(Func<int, T, int> formula, IEnumerable<T> values, IEnumerable<TeamBase>? targets) where T : INumber<T>
	{
		targets ??= AliveTeams;

		foreach (var (target, value) in Enumerable.Zip(targets, values))
		{
            target.Population = formula(target.Population, value);
		}

		Stepped();
	}

    #endregion Auxiliary

    #region Methods

    public void Multiply(double rate, IEnumerable<TeamBase>? targets = null)
    {
        static int MultiplyFormula(int population, double rate)
            => (int)(population * rate);

        ForeachTeam(MultiplyFormula, rate, targets);
    }

    public void Multiply(double min, double max, IEnumerable<TeamBase>? targets = null)
    {
         static int MultiplyFormula(int population, double min, double max)
             => (int)(population * Random.Shared.NextDouble(min, max));
     
         ForeachTeam(MultiplyFormula, min, max, targets);
    }

    public void MultiplyWithMeanAndStdev(double mean, double stdev, IEnumerable<TeamBase>? targets = null)
    {
		static int MultiplyFormula(int population, double rate)
			=> (int)(population * rate);

		targets ??= AliveTeams;
        IEnumerable<double> rates = Random.Shared.NextDoubles(mean, stdev, targets.Count());

        ForeachTeam(MultiplyFormula, rates, targets);
        }

        Stepped();
    }

    public void Add(int amount, IEnumerable<TeamBase>? targets = null)
    {
        static int AdditionFormula(int population, int amount)
            => population + amount;

        ForeachTeam(AdditionFormula, amount, targets);
    }

    public void Add(int min, int max, IEnumerable<TeamBase>? targets = null)
    {
        static int AdditionFormula(int population, int min, int max)
            => population + Random.Shared.Next(min, max);

        ForeachTeam(AdditionFormula, min, max, targets);
    }

    public void AddWithMeanAndStdev(double mean, double stdev, IEnumerable<TeamBase>? targets = null)
    {
		static int AdditionFormula(int population, int amount)
			=> population + amount;

        targets ??= AliveTeams;
		IEnumerable<int> amounts = Random.Shared.NextInts(mean, stdev, targets.Count());

		ForeachTeam(AdditionFormula, amounts, targets);
        }

        Stepped();
    }

    public void Set(int amount, IEnumerable<TeamBase>? targets = null)
    {
        static int SetFormula(int population, int amount)
            => amount;

        ForeachTeam(SetFormula, amount, targets);
    }

    public void Set(int min, int max, IEnumerable<TeamBase>? targets = null)
    {
        static int SetFormula(int population, int min, int max)
            => Random.Shared.Next(min, max);

        ForeachTeam(SetFormula, min, max, targets);
    }

    public void SetWithMeanAndStdev(double mean, double stdev, IEnumerable<TeamBase>? targets = null)
    {
		static int SetFormula(int population, int amount)
			=> amount;

		targets ??= AliveTeams;
        IEnumerable<int> amounts = Random.Shared.NextInts(mean, stdev, targets.Count());

		ForeachTeam(SetFormula, amounts, targets);
        {
            target.Population = amount;
        }

        Stepped();
    }

    public void Swap(TeamBase a, TeamBase b)
    {
        (a.Population, b.Population) = (b.Population, a.Population);
        Stepped();
    }

    public void Shuffle(IEnumerable<TeamBase>? targets = null)
    {
        List<TeamBase> teams = new(targets ?? AliveTeams); 

        for (int i = 0; i < teams.Count - 2; i++)
        {
            int j = Random.Shared.Next(i, teams.Count);
            TeamBase a = teams[i];
            TeamBase b = teams[j];

            (a.Population, b.Population) = (b.Population, a.Population);
        }

        Stepped();
    }
    #endregion Methods

    #region params overloads

    public void Multiply(double rate, params TeamBase[] targets)
        => Multiply(rate, targets.AsEnumerable());

    public void Multiply(double min, double max, params TeamBase[] targets)
        => Multiply(min, max, targets.AsEnumerable());

    public void Add(int amount, params TeamBase[] targets)
        => Add(amount, targets.AsEnumerable());

    public void Add(int min, int max, params TeamBase[] targets)
        => Add(min, max, targets.AsEnumerable());

    public void Set(int amount, params TeamBase[] targets)
        => Set(amount, targets.AsEnumerable());

    public void Set(int min, int max, params TeamBase[] targets)
        => Set(min, max, targets.AsEnumerable());

    #endregion params overloads
}
