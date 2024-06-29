namespace Maiswan.Marble;

public record class ConfigRoot
{
    public Options Options { get; set; } = new();

    public List<Team> Teams { get; set; } = [];

    public List<Round> Rounds { get; set; } = [];
}
