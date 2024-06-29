using System.Text.Json;

namespace Maiswan.Marble;

internal class Program
{
    private static string GetJsonPath(string[] commandLineArgs)
    {
        string? jsonPath = ".json";
        if (commandLineArgs.Length > 0) { jsonPath = commandLineArgs[0]; }

        while (!File.Exists(jsonPath))
        {
            Console.Write("Path > ");
            jsonPath = Console.ReadLine();
        }

        return jsonPath;
    }

    private static ConfigRoot GetConfigFromJsonPath(string path)
    {
        string json = File.ReadAllText(path);
        ConfigRoot config = JsonSerializer.Deserialize<ConfigRoot>(json) ?? new();

        return config;
    }

    static void Main(string[] args)
    {
        string path = GetJsonPath(args);
        ConfigRoot config = GetConfigFromJsonPath(path);

        MarbleGameView view = new(config.Teams, config.Options.ScriptPath)
        {
            DeathIfFewer = config.Options.DeathIfFewer,
            DelayBetweenRounds = config.Options.DelayBetweenRounds,
            DisplayPercentage = config.Options.DisplayPercentage,
            DisplayZero = config.Options.DisplayZero,
        };
        view.Run();
    }
}
