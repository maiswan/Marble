using NLua;
using NLua.Exceptions;
using System.Reflection;

namespace Maiswan.Marble;

internal class MarbleGame
{
    public int DeathIfFewer { get; set; }
    public event EventHandler<MarbleGameChangedEventArgs>? GameStepped;
    public event EventHandler<MarbleGameChangedEventArgs>? GameEnded;

    public int TotalPopulation => teams.Sum(x => x.Population);
    public int TeamsAlive => teams.Count(x => x.Population != 0);
    public IReadOnlyCollection<Team> Teams => teams.AsReadOnly();

    private readonly List<Team> teams;

    private int iteration;

    private readonly Lua lua = new();
    public string LuaFile { get; init; } = "";

    public MarbleGame(IEnumerable<Team> teams)
    {
        this.teams = new(teams);

        // basic sandboxing
        if (!lua.IsExecuting) { InitializeLuaSandbox(); }
    }

    private void InitializeLuaSandbox()
    {
        lua["MarbleGame"] = this;

        // Execute Sandbox.lua as an embdded resource
        Assembly executing = Assembly.GetExecutingAssembly();
        string? name = executing.GetName().Name;

        using Stream? stream = executing.GetManifestResourceStream($"{name}.Sandbox.lua");
        if (stream is null) { return; }

        StreamReader streamReader = new(stream);
        string sandbox = streamReader.ReadToEnd();
        lua.DoString(sandbox);
    }

    public void Run()
    {
        if (lua.IsExecuting) { return; }

        RaiseGameChangedEvent(false);

        // Invoke user script through sandbox
        try
        {
            string user = File.ReadAllText(LuaFile);
            LuaFunction sandbox = (LuaFunction)lua["run_sandbox"];
            sandbox.Call(user);
        }
        catch ( LuaException ) { }

        RaiseGameChangedEvent(true);
    }

    private void RaiseGameChangedEvent(bool isGameOver)
    {
        MarbleGameChangedEventArgs e = new(isGameOver, iteration, Teams);
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

    private Team SetPopulation(Team team, int population)
    {
        if (population < DeathIfFewer) { population = 0; }
        return team with { PreviousPopulation = team.Population, Population = population };
    }

    public void Multiply(double min, double max)
    {
        for (int i = 0; i < teams.Count; i++)
        {
            Team team = teams[i];

            if (team.Population == 0)
            {
                teams[i] = SetPopulation(team, 0);
                continue;
            }

            double deathRate = Random.Shared.NextDouble(min, max);
            int newPopulation = (int)(team.Population * deathRate);
            teams[i] = SetPopulation(team, newPopulation);
        }

        CheckEnd();
    }

    public void Add(int min, int max)
    {
        for (int i = 0; i < teams.Count; i++)
        {
            Team team = teams[i];

            if (team.Population == 0)
            {
                teams[i] = SetPopulation(team, 0); 
                continue;
            }

            int delta = Random.Shared.Next(min, max);
            teams[i] = SetPopulation(team, team.Population + delta);
        }

        CheckEnd();
    }

    public void Set(int teamIndex, int population)
    {
        teams[teamIndex] = SetPopulation(teams[teamIndex], population);
        CheckEnd();
    }
}
