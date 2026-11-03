using NUnit.Framework;
using Core;

public class ProductionRulesTests
{
    private static void AddPoints(GameState state, int residentId, string target, int points)
    {
        for (int i = 0; i < points; i++)
            state.Assignments.Add(residentId, target);
    }

    [Test]
    public void Produced_WithoutTrait_ThreePointsOnFarmGiveTwentyFourFood()
    {
        var state = TestHelpers.MakeState();
        AddPoints(state, 0, "farm", 3);

        Assert.AreEqual(24, ProductionRules.Produced(state, ResourceType.Food));
        Assert.AreEqual(0, ProductionRules.Produced(state, ResourceType.Water));
    }

    [Test]
    public void Produced_Farmer_GetsTwentyFivePercentMoreFood()
    {
        var state = TestHelpers.MakeState();
        TestHelpers.SetTrait(state, 0, "farmer");
        AddPoints(state, 0, "farm", 3);

        Assert.AreEqual(30, ProductionRules.Produced(state, ResourceType.Food));
    }

    [Test]
    public void Produced_Waterbearer_BonusOnlyForWater()
    {
        var state = TestHelpers.MakeState();
        TestHelpers.SetTrait(state, 0, "waterbearer");
        AddPoints(state, 0, "farm", 3);
        AddPoints(state, 1, "well", 3);
        TestHelpers.SetTrait(state, 1, "waterbearer");

        Assert.AreEqual(24, ProductionRules.Produced(state, ResourceType.Food)); // brak premii przy uprawie
        Assert.AreEqual(30, ProductionRules.Produced(state, ResourceType.Water));
    }

    [Test]
    public void Produced_SumsPointsOfDifferentResidents()
    {
        var state = TestHelpers.MakeState();
        AddPoints(state, 0, "farm", 3);
        AddPoints(state, 1, "farm", 2);

        Assert.AreEqual(40, ProductionRules.Produced(state, ResourceType.Food));
    }

    [Test]
    public void Produced_RestPointsProduceNothing()
    {
        var state = TestHelpers.MakeState();
        AddPoints(state, 0, AssignmentPlan.RestId, 3);

        Assert.AreEqual(0, ProductionRules.Produced(state, ResourceType.Food));
        Assert.AreEqual(0, ProductionRules.Produced(state, ResourceType.Water));
    }
}
