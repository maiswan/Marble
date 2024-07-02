using NLua;
using NLua.Exceptions;
using System.Numerics;
using System.Reflection;

namespace Maiswan.Marble;

public class MarbleGame(IEnumerable<Team> teams)
{
    public int DeathIfFewer { get; set; }
    public event EventHandler<MarbleGameChangedEventArgs>? GameStepped;
    public event EventHandler<MarbleGameChangedEventArgs>? GameEnded;

    public int TotalPopulation => teams.Sum(x => x.Population);
    public int TeamsAlive => teams.Count(x => x.Population != 0);
    public IReadOnlyCollection<Team> Teams => teams.AsReadOnly();

    private readonly List<Team> teams = new(teams);

    private int iteration;

    private readonly Lua lua = new();
    public string LuaFile { get; init; } = "";

    public void Run()
    {
        if (lua.IsExecuting) { return; }

        RaiseGameChangedEvent(false);

        // Invoke user script through sandbox
        try
        {
            InitializeLuaSandbox();
            InvokeLua();
        }
        catch ( LuaException ) { }

        RaiseGameChangedEvent(true);
    }

    private void InitializeLuaSandbox()
    {
        lua["MarbleGame"] = this;

        // Execute Sandbox.lua as an embdded resource
        Assembly executing = Assembly.GetExecutingAssembly();
        string? name = executing.GetName().Name;

        using Stream? stream = executing.GetManifestResourceStream($"{name}.Sandbox.lua");
        if (stream is null) { throw new IOException(nameof(stream)); }

        StreamReader streamReader = new(stream);
        string sandbox = streamReader.ReadToEnd();
        lua.DoString(sandbox);
    }

    private void InvokeLua()
    {
        string user = File.ReadAllText(LuaFile);
        LuaFunction sandbox = (LuaFunction)lua["run_sandbox"];
        sandbox.Call(user);
    }

    private void RaiseGameChangedEvent(bool isGameOver)
    {
        MarbleGameChangedEventArgs e = new()
        {
            IsGameOver = isGameOver,
            Iteration = iteration,
            Teams = teams,
        };
        EventHandler<MarbleGameChangedEventArgs>? handler = isGameOver ? GameEnded : GameStepped;
        handler?.Invoke(this, e);
    }

    private void CheckEnd()
    {
        iteration++;

        if (teams.Any(x => x.Population != 0))
        {
            RaiseGameChangedEvent(false);
            return;
        }

        // Halt the script via an exception
        lua.State.Error();
    }

    private void ForeachTeam<T>(Func<int, T, T, int> formula, T min, T max) where T : INumber<T>
    {
        for (int i = 0; i < teams.Count; i++)
        {
            teams[i] = SetPopulation(
                teams[i],
                teams[i].Population == 0
                    ? 0
                    : formula(teams[i].Population, min, max)
            );
        }
    }
    private Team SetPopulation(Team team, int population)
    {
        if (population < DeathIfFewer) { population = 0; }
        return team with { PreviousPopulation = team.Population, Population = population };
    }

    public void Multiply(double min, double max)
    {
        static int MultiplyFormula(int population, double min, double max)
            => (int)(population * Random.Shared.NextDouble(min, max));

        ForeachTeam(MultiplyFormula, min, max);
        CheckEnd();
    }

    public void Add(int min, int max)
    {
        static int AddFormula(int population, int min, int max)
            => population + Random.Shared.Next(min, max);

        ForeachTeam(AddFormula, min, max);
        CheckEnd();
    }

    public void Set(int teamIndex, int population)
    {
        teams[teamIndex] = SetPopulation(teams[teamIndex], population);
        CheckEnd();
    }
}
