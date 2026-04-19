namespace Roll.Randomizer;

public class MinMaxRandomizer : IRandomizer
{
    public int Min { get; }
    public int Max { get; }
    public string Name { get; }

    public MinMaxRandomizer(int min = 1, int max = 100)
    {
        Min = min;
        Max = max;

        Name = $"{Min}-{Max}";
    }

    public string GetRandomValue()
    {
        var value = RollModSystem.Random.Next(Min, Max + 1);
        return value.ToString();
    }
}