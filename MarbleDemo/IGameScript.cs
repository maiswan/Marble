namespace Maiswan.Marble.Demo;

public interface IGameScript
{
    public void Initialize(params object[] parameters);

    public void Run(MarbleGame game);//, CancellationToken token);

    public void OnGameStepped(object? sender, MarbleGameChangedEventArgs e);

    public void OnGameEnded(object? sender, MarbleGameChangedEventArgs e);
}
