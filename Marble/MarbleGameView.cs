namespace Maiswan.Marble;

internal class MarbleGameView
{
    public int DelayBetweenRounds { get; set; }
    
    public bool DisplayPercentage { get; set; }
    
    public bool DisplayZero { get; set; }
    
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
    }

    public void Run()
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        game.Run();
        Console.ForegroundColor = originalColor;
    }

    private void OnGameStepped(object? sender, MarbleGameChangedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("#{0,3}", e.Iteration);

        string format = DisplayPercentage switch
        {
            true => "{0,5:0}",
            false when DisplayZero => "{0,6}",
            false when !DisplayZero => "{0,6:#;;}",
            _ => throw new InvalidOperationException(),
        };

        foreach (Team team in e.Teams)
        {
            double output = team.Population;
            if (DisplayPercentage)
            {
                output *= 100d / e.TotalPopulation;
            }

            Console.ForegroundColor = team.Color;
            Console.Write(format, output);
        }

        if (DelayBetweenRounds > 0)
        {
            Thread.Sleep(DelayBetweenRounds);
            Console.WriteLine();
            return;
        }
        if (DelayBetweenRounds < 0)
        {
            Console.ReadLine();
        }
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