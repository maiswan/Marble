using NLua;
using NLua.Exceptions;
using System.Reflection;

namespace Maiswan.Marble.Demo;

internal class MarbleGameView
{
    private int delayBetweenRounds;
    internal int DelayBetweenRounds
    {
        get => delayBetweenRounds;
        set
        {
            delayBetweenRounds = value;
            UpdateWriteFormat();
        }
    }

    private bool displayPercentage;
    internal bool DisplayPercentage
    {
        get => displayPercentage;
        set
        {
            displayPercentage = value;
            UpdateWriteFormat();
        }
    }

    private bool displayZero;
    internal bool DisplayZero
    {
        get => displayZero;
        set
        {
            displayZero = value;
            UpdateWriteFormat();
        }
    }
    
    private int deathIfFewer;
    internal int DeathIfFewer
    {
        get => deathIfFewer;
        set
        {
            deathIfFewer = value;
            game.DeathIfFewer = value;
        }
    }

    private string writeFormat = "";

    private readonly MarbleGame game;

    private ConsoleColor originalColor;

    private readonly Lua lua = new();

    private readonly string luaFile;

    internal MarbleGameView(IList<DemoTeam> teams, string scriptPath)
    {
        game = new(teams)
        {
            DeathIfFewer = deathIfFewer,
        };
        game.GameStepped += OnGameStepped;
        game.GameEnded += OnGameEnded;
        UpdateWriteFormat();

        luaFile = scriptPath;
    }
    
    private void InitializeLuaSandbox()
    {
        lua["MarbleGame"] = game;
    
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
        string user = File.ReadAllText(luaFile);
        LuaFunction sandbox = (LuaFunction)lua["run_sandbox"];
        sandbox.Call(user);
    }

    internal void Run()
    {
        originalColor = Console.ForegroundColor;
        Console.CancelKeyPress += Console_CancelKeyPress;

        try
        {
            InitializeLuaSandbox();
            InvokeLua();
        }
        catch (LuaException) { }

        Console.ForegroundColor = originalColor;
    }

    private void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        Console.ForegroundColor = originalColor;
    }

    private void UpdateWriteFormat()
    {
        writeFormat = DisplayPercentage switch
        {
            true => "{0,5:0}",
            false when DisplayZero => "{0,6}",
            false => "{0,6:#;;}",
        };
    }

    private void OnGameStepped(object? sender, MarbleGameChangedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("#{0,3}", e.Iteration);

        foreach (TeamBase team in e.Teams)
        {
            double output = DisplayPercentage
                ? team.Population * 100d / e.TotalPopulation
                : team.Population;

            Console.ForegroundColor = ((DemoTeam)team).Color;
            Console.Write(writeFormat, output);
        }

        Interval();
    }

    private void Interval()
    {
        if (DelayBetweenRounds == 0) { return; }

        if (DelayBetweenRounds < 0)
        {
            Console.ReadLine();
            return;
        }

        Thread.Sleep(DelayBetweenRounds);
        Console.WriteLine();
    }

    private void OnGameEnded(object? sender, MarbleGameChangedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Gray;

        int max = e.Teams.Max(x => x.PreviousPopulation);
        IEnumerable<TeamBase> winners = e.Teams.Where(x => x.PreviousPopulation == max);

        Console.Write("Winning: ");
        foreach (TeamBase team in winners)
        {
            Console.ForegroundColor = ((DemoTeam)team).Color;
            Console.Write("{0} ", ((DemoTeam)team).Name);
        }

        // Halt script
        lua.State.Error();
    }
}