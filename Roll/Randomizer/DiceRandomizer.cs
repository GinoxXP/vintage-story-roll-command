namespace Roll.Randomizer;

public class DiceRandomizer : IRandomizer
{
    public int Min { get; } = 1;
    public int Max { get; }
    public string Name { get; }

    public DiceRandomizer(int max)
    {
        Max = max;
        Name = $"D{max}";
    }
    
    public string GetRandomValue()
    {
        var value = RollModSystem.Random.Next(Min, Max + 1);
        
        if (value == Min)
            return $"<font color=#ff4400>{value}</font>";
        
        if (value == Max)
            return $"<font color=#55ff55>{value}</font>";
        
        return value.ToString();
    }
}