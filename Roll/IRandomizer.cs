namespace Roll;

public interface IRandomizer
{
    public int Min { get; }
    
    public int Max { get; }
    
    public string Name { get; }
    public string GetRandomValue();
}