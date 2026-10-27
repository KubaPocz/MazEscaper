using NUnit.Framework;

public class SeededRandomTests
{
    [Test]
    public void SameSeed_GivesSameSequence()
    {
        var a = new SeededRandom(12345);
        var b = new SeededRandom(12345);
        for (int i = 0; i < 100; i++)
            Assert.AreEqual(a.NextInt(0, 1000), b.NextInt(0, 1000));
    }

    [Test]
    public void DifferentSeeds_GiveDifferentSequences()
    {
        var a = new SeededRandom(1);
        var b = new SeededRandom(2);
        bool anyDifferent = false;
        for (int i = 0; i < 20; i++)
            if (a.NextInt(0, 1000) != b.NextInt(0, 1000)) anyDifferent = true;
        Assert.IsTrue(anyDifferent);
    }

    [Test]
    public void NextInt_StaysInRange()
    {
        var r = new SeededRandom(7);
        for (int i = 0; i < 1000; i++)
        {
            int v = r.NextInt(3, 8);
            Assert.GreaterOrEqual(v, 3);
            Assert.Less(v, 8);
        }
    }

    [Test]
    public void Chance_ZeroNeverAndOneAlways()
    {
        var r = new SeededRandom(99);
        for (int i = 0; i < 100; i++)
        {
            Assert.IsFalse(r.Chance(0f));
            Assert.IsTrue(r.Chance(1f));
        }
    }
}