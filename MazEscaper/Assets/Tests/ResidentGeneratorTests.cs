using System.Linq;
using NUnit.Framework;
using Core;

public class ResidentGeneratorTests
{
    [Test]
    public void Generate_ReturnsRequestedCountWithUniqueNamesAndTraits()
    {
        var residents = ResidentGeneratorSystem.Generate(5, GameContent.CreatePrototype(), new SeededRandom(7));

        Assert.AreEqual(5, residents.Count);
        Assert.AreEqual(5, residents.Select(r => r.Name).Distinct().Count());
        Assert.IsTrue(residents.All(r => r.Trait != null));
    }

    [Test]
    public void Generate_AlwaysCoversAtLeastThreeDomains()
    {
        var content = GameContent.CreatePrototype();

        for (int seed = 1; seed <= 200; seed++)
        {
            var residents = ResidentGeneratorSystem.Generate(5, content, new SeededRandom(seed));
            int domains = residents.Select(r => r.Trait.Domain).Distinct().Count();
            Assert.GreaterOrEqual(domains, 3, "seed " + seed);
        }
    }

    [Test]
    public void Generate_SameSeed_GivesSameResidents()
    {
        var content = GameContent.CreatePrototype();
        var a = ResidentGeneratorSystem.Generate(5, content, new SeededRandom(42));
        var b = ResidentGeneratorSystem.Generate(5, content, new SeededRandom(42));

        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(a[i].Name, b[i].Name);
            Assert.AreEqual(a[i].Trait.Id, b[i].Trait.Id);
        }
    }

    [Test]
    public void NewGameState_HasResidentsFromConfig()
    {
        var state = new GameState(new GameConfig(), new SeededRandom(3));

        Assert.AreEqual(state.Config.StartingResidentCount, state.ResidentCount);
        Assert.AreEqual(state.ResidentCount, state.Residents.Count);
    }
}
