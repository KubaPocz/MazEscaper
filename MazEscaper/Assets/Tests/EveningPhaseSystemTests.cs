using NUnit.Framework;
using Core;

public class EveningPhaseSystemTests
{
    [Test]
    public void CalculateReport_DoesNotChangeStorehouse()
    {
        var state = TestHelpers.MakeState();

        var first = EveningPhaseSystem.CalculateReport(state);
        var second = EveningPhaseSystem.CalculateReport(state);

        Assert.AreEqual(80, state.Storehouse.Food.CurrentAmount);
        Assert.AreEqual(80, state.Storehouse.Water.CurrentAmount);
        Assert.AreEqual(first.UsedFood, second.UsedFood);
        Assert.AreEqual(first.SpoiledFood, second.SpoiledFood);
        Assert.AreEqual(first.UsedWater, second.UsedWater);
    }

    [Test]
    public void ApplyPhase_RemovesResourcesAndStoresReport()
    {
        var state = TestHelpers.MakeState(80, 80);

        EveningPhaseSystem.ApplyPhase(state);

        Assert.AreEqual(57, state.Storehouse.Food.CurrentAmount);
        Assert.AreEqual(60, state.Storehouse.Water.CurrentAmount);

        var report = state.EveningReport;
        Assert.IsNotNull(report);
        Assert.AreEqual(20, report.NeededFood);
        Assert.AreEqual(20, report.UsedFood);
        Assert.AreEqual(3, report.SpoiledFood);
        Assert.AreEqual(20, report.NeededWater);
        Assert.AreEqual(20, report.UsedWater);
    }

    [TestCase(80, 20, 3, 57)]   // psucie liczone od 60 (po zużyciu), a nie od 80
    [TestCase(40, 20, 1, 19)]   // 5% z 20 = 1
    [TestCase(25, 20, 0, 5)]    // 5% z 5 = 0
    [TestCase(16, 16, 0, 0)]    // niedobór: zużyto tylko tyle, ile było
    [TestCase(0, 0, 0, 0)]      // pusty magazyn
    public void ApplyPhase_FoodUseAndSpoilage(int start, int used, int spoiled, int remaining)
    {
        var state = TestHelpers.MakeState(food: start);

        EveningPhaseSystem.ApplyPhase(state);

        Assert.AreEqual(used, state.EveningReport.UsedFood);
        Assert.AreEqual(spoiled, state.EveningReport.SpoiledFood);
        Assert.AreEqual(remaining, state.Storehouse.Food.CurrentAmount);
    }

    [Test]
    public void ApplyPhase_WaterShortage_UsesOnlyWhatIsAvailable()
    {
        var state = TestHelpers.MakeState(water: 10);

        EveningPhaseSystem.ApplyPhase(state);

        Assert.AreEqual(20, state.EveningReport.NeededWater);
        Assert.AreEqual(10, state.EveningReport.UsedWater);
        Assert.AreEqual(10, state.EveningReport.NeededWater - state.EveningReport.UsedWater); // brak
        Assert.AreEqual(0, state.Storehouse.Water.CurrentAmount);
    }

    [Test]
    public void ApplyPhase_FourEveningsInARow_MatchesExpectedStock()
    {
        var state = TestHelpers.MakeState(80, 80);
        int[] expectedFood = { 57, 36, 16, 0 };
        int[] expectedWater = { 60, 40, 20, 0 };

        for (int i = 0; i < 4; i++)
        {
            EveningPhaseSystem.ApplyPhase(state);
            Assert.AreEqual(expectedFood[i], state.Storehouse.Food.CurrentAmount, "żywność, wieczór " + (i + 1));
            Assert.AreEqual(expectedWater[i], state.Storehouse.Water.CurrentAmount, "woda, wieczór " + (i + 1));
        }
    }

    [Test]
    public void AdvancingPhases_RunsEveningOnlyWhenEnteringEvening()
    {
        var state = TestHelpers.MakeState(80, 80);
        var command = new AdvancePhaseCommand();

        command.Execute(state); // Poranek -> Dzień
        Assert.IsNull(state.EveningReport);
        Assert.AreEqual(80, state.Storehouse.Food.CurrentAmount);

        command.Execute(state); // Dzień -> Wieczór
        Assert.IsNotNull(state.EveningReport);
        Assert.AreEqual(57, state.Storehouse.Food.CurrentAmount);
        Assert.AreEqual(60, state.Storehouse.Water.CurrentAmount);

        command.Execute(state); // Wieczór -> Poranek (magazyn bez zmian)
        Assert.AreEqual(57, state.Storehouse.Food.CurrentAmount);
        Assert.AreEqual(2, state.CurrentDay);
    }
}