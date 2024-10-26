using System.Text.Json;

namespace Maiswan.Marble.Demo;

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
        return JsonSerializer.Deserialize<ConfigRoot>(json) ?? new();
    }

    internal static void Main(string[] args)
    {
        string path = GetJsonPath(args);
        ConfigRoot config = GetConfigFromJsonPath(path);

        new MarbleGameController(config.Teams, config.Options).Run();
    }
}
