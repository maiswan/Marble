using CSScriptLib;

namespace Maiswan.Marble.Demo;

internal class MarbleGameController
{
    private readonly MarbleGame game;

    private readonly IGameScript script;

    internal MarbleGameController(IList<DemoTeam> teams, MarbleGameOptions options)
    {
        game = new(teams)
        {
            DeathIfFewer = options.DeathIfFewer,
        };

        string user = File.ReadAllText(options.ScriptPath);
        script = CSScript.Evaluator.LoadCode<IGameScript>(user);
        script.Initialize(options);

        game.GameStepped += script.OnGameStepped;   // handover output process to client
        game.GameEnded += script.OnGameEnded;   // handover output process to client
    }

    internal void Run()
    {
        CancellationTokenSource cts = new();
        game.GameEnded += (object? sender, MarbleGameChangedEventArgs e) => cts.Cancel();

        ConsoleColor originalColor = Console.ForegroundColor;
        Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) => Console.ForegroundColor = originalColor;

        try
        {
            Task task = new(() => script.Run(game));
            task.Start();
            task.Wait(cts.Token);
        }
        catch (OperationCanceledException) { }

        Console.ForegroundColor = originalColor;
    }
}