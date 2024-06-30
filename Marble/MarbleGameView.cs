namespace Maiswan.Marble;

internal class MarbleGameView
{
    private int delayBetweenRounds;
    public int DelayBetweenRounds
    {
        get => delayBetweenRounds;
        set
        {
            delayBetweenRounds = value;
            UpdateWriteFormat();
        }
    }

    private bool displayPercentage;
    public bool DisplayPercentage
    {
        get => displayPercentage;
        set
        {
            displayPercentage = value;
            UpdateWriteFormat();
        }
    }

    private bool displayZero;
    public bool DisplayZero
    {
        get => displayZero;
        set
        {
            displayZero = value;
            UpdateWriteFormat();
        }
    }
    
    private int deathIfFewer;
    public int DeathIfFewer
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

    public MarbleGameView(IEnumerable<Team> teams, string scriptPath)
    {
        game = new(teams)
        {
            DeathIfFewer = deathIfFewer,
            LuaFile = scriptPath,
        };
        game.GameStepped += OnGameStepped;
        game.GameEnded += OnGameEnded;
        UpdateWriteFormat();
    }

    public void Run()
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        game.Run();
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

        foreach (Team team in e.Teams)
        {
            double output = DisplayPercentage
                ? team.Population * 100d / e.TotalPopulation
                : team.Population;

            Console.ForegroundColor = team.Color;
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
        IEnumerable<Team> winners = e.Teams.Where(x => x.PreviousPopulation == max);

        Console.Write("Winning: ");
        foreach (Team team in winners)
        {
            Console.ForegroundColor = team.Color;
            Console.Write("{0} ", team.Name);
        }
    }
}