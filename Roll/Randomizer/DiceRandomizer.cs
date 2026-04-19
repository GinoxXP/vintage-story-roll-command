namespace Roll.Randomizer;

public class DiceRandomizer : IRandomizer
{
    private readonly int _count;
    public int Min { get; } = 1;
    public int Max { get; }
    public string Name { get; }
    public DiceRandomizer(int sides, int count = 1)
    {
        Max = sides;
        Name = count == 1
            ? $"D{Max}"
            : $"{count}D{Max}";
        
        _count = count;
    }
    
    public string GetRandomValue()
    {
        var results = new string[_count];
        
        for (int i = 0; i < _count; i++)
            results[i] = Roll();
        
        return string.Join(", ", results);
    }

    private string Roll()
    {
        var value = RollModSystem.Random.Next(Min, Max + 1);
        
        if (value == Min)
            return $"<font color=#ff4400>{value}</font>";
        
        if (value == Max)
            return $"<font color=#55ff55>{value}</font>";
        
        return value.ToString();
    }
}