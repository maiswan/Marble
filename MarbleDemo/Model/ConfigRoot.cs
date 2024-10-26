namespace Maiswan.Marble.Demo;

public record class ConfigRoot
{
    public MarbleGameOptions Options { get; set; }

    public List<DemoTeam> Teams { get; set; } = [];
}
