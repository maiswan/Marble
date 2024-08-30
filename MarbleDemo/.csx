using Maiswan.Marble;
using Maiswan.Marble.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Script : IGameScript
{
    private MarbleGameOptions options;
    private string writeFormat = "";
    private bool isGameOver;

    public void Initialize(params object[] parameters)
    {
        if (parameters.Length == 0 || parameters[0] is not MarbleGameOptions options)
        {
            throw new ArgumentException(nameof(parameters));
        }

        this.options = options;
        
        writeFormat = options.DisplayPercentage switch
        {
            true => "{0,5:0}",
            false when options.DisplayZero => "{0,6}",
            false => "{0,6:#;;}",
        };
    }

    public void Run(MarbleGame game)
    {
        for (int i = 0; i < 4; i++)
        {
            game.MultiplyWithMeanAndStdev(1.1, 0.5);
            game.Shuffle(game.AliveTeams);
        }

        while (true)
        {
            game.Multiply(0.7, 1.1);

            // bonus
            if (game.AliveTeams.Count >= 2 && game.TotalPopulation <= 100)
            {
                int index = Random.Shared.Next(game.AliveTeams.Count);
                game.Multiply(1.5, 1.5 + game.AliveTeams[index].Population / game.TotalPopulation, game.AliveTeams[index]);
            }

            // tenbatsu
            double draw = Random.Shared.NextDouble();
            if (draw <= 0.04 && game.AliveTeams.Count > 0)
            {
                int index = Random.Shared.Next(game.AliveTeams.Count);
                game.Set(5, 10, game.AliveTeams[index]);
            }

            // end game quicker
            if (game.AliveTeams.Count <= 2)
            {
                game.Multiply(0.8, 1.0);
            }
            if (game.AliveTeams.Count == 1)
            {
                game.Multiply(0.4, 1.0);
            }
        }
    }

    public void OnGameStepped(object? sender, MarbleGameChangedEventArgs e)
    {
        // sorry but this prevents race condition
        if (e.IsGameOver) { return; }

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("#{0,3}", e.Iteration);
        
        foreach (TeamBase team in e.Teams)
        {
            double output = options.DisplayPercentage
                ? team.Population * 100d / e.TotalPopulation
                : team.Population;
        
            Console.ForegroundColor = ((DemoTeam)team).Color;
            Console.Write(writeFormat, output);
        }

        Console.WriteLine();

        Interval();
    }

    private void Interval()
    {
        if (options.DelayBetweenRounds < 0) { Console.ReadLine(); }
        if (options.DelayBetweenRounds <= 0) { return; }

        Thread.Sleep(options.DelayBetweenRounds);
    }

    public void OnGameEnded(object? sender, MarbleGameChangedEventArgs e)
    {
        // only show the game over message once
        if (e.IsGameOver && isGameOver) { return; }
        isGameOver = e.IsGameOver;

        Console.ForegroundColor = ConsoleColor.Gray;
        
        int max = e.Teams.Max(x => x.PreviousPopulation);
        IEnumerable<TeamBase> winners = e.Teams.Where(x => x.PreviousPopulation == max);
        
        Console.Write("Winning: ");
        foreach (TeamBase team in winners)
        {
            Console.ForegroundColor = ((DemoTeam)team).Color;
            Console.Write("{0} ", ((DemoTeam)team).Name);
        }

        Console.WriteLine();
    }
}