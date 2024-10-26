namespace Maiswan.Marble.Demo;

public readonly struct MarbleGameOptions
{
    public int DelayBetweenRounds { get; init; }
    public bool DisplayPercentage { get; init; }
    public bool DisplayZero { get; init; }
    public int DeathIfFewer { get; init; }
    public string ScriptPath { get; init; } = "";

    public MarbleGameOptions()
    { }
}
