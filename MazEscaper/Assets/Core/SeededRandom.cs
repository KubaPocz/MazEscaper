using MazEscaper.Core;
using Random = System.Random;

public class SeededRandom : IRandom
{
    private readonly Random rng;

    public int Seed { get; }

    public SeededRandom(int seed)
    {
        Seed = seed;
        rng = new Random(seed);
    }

    public int NextInt(int minInclusive, int maxExclusive) => rng.Next(minInclusive, maxExclusive);
    public float NextFloat() => (float)rng.NextDouble();
    public bool Chance(float probability) => rng.NextDouble() < probability;

}