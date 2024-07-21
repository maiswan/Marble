namespace Maiswan.Marble.Demo;

public record class ConfigRoot
{
    public Options Options { get; set; } = new();

    public List<DemoTeam> Teams { get; set; } = [];
}
