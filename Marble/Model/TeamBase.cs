namespace Maiswan.Marble;

public abstract class TeamBase
{
    public int PreviousPopulation { get; private set; }

    private int population;
    public int Population
    {
        get => population;
        set
        {
            PreviousPopulation = population;
            population = value;
        }
    }
}