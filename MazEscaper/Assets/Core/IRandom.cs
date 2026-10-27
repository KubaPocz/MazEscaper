
namespace MazEscaper.Core
{
    public interface IRandom
    {
        int Seed { get; }
        int NextInt(int minInclusive, int maxExclusive);
        float NextFloat();
        bool Chance(float propability);
    }
}
