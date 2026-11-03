using NUnit.Framework;
using Core;

public class ConsumptionRulesTests
{
    [Test]
    public void Needed_WithoutTraits_IsFourPerResident()
    {
        var state = TestHelpers.MakeState();

        Assert.AreEqual(20, ConsumptionRules.NeededFood(state));
        Assert.AreEqual(20, ConsumptionRules.NeededWater(state));
    }

    [Test]
    public void Needed_FrugalResident_NeedsThreeInsteadOfFour()
    {
        var state = TestHelpers.MakeState();
        TestHelpers.SetTrait(state, 0, "frugal");

        Assert.AreEqual(19, ConsumptionRules.NeededFood(state));
        Assert.AreEqual(19, ConsumptionRules.NeededWater(state));
    }

    [Test]
    public void StockAfterProduction_IsClampedToCapacity()
    {
        Assert.AreEqual(104, ConsumptionRules.StockAfterProduction(80, 24, 400));
        Assert.AreEqual(400, ConsumptionRules.StockAfterProduction(395, 24, 400));
    }

    [Test]
    public void FoodAfterDay_ProductionThenUseThenSpoilage()
    {
        var state = TestHelpers.MakeState();

        // 80 + 24 = 104, zużycie 20 -> 84, psucie floor(4.2) = 4 -> 80
        Assert.AreEqual(80, ConsumptionRules.FoodAfterDay(80, 24, 20, 400, state.Config));
    }

    [Test]
    public void SpoiledFood_BoundaryValues()
    {
        var config = TestHelpers.MakeState().Config;

        Assert.AreEqual(0, ConsumptionRules.SpoiledFood(0, config));
        Assert.AreEqual(0, ConsumptionRules.SpoiledFood(19, config));
        Assert.AreEqual(1, ConsumptionRules.SpoiledFood(20, config));
        Assert.AreEqual(5, ConsumptionRules.SpoiledFood(100, config));
        Assert.AreEqual(20, ConsumptionRules.SpoiledFood(400, config));
    }
}
