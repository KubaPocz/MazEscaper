using NUnit.Framework;
using Core;

public class EveningProductionTests
{
    private static void AssignThreeToFarm(GameState state, int residentId = 0)
    {
        state.Phase = Phase.Day;
        for (int i = 0; i < 3; i++)
            new AssignPointCommand(residentId, "farm").Execute(state);
    }

    [Test]
    public void ApplyPhase_ProductionIsAddedBeforeConsumptionAndSpoilage()
    {
        var state = TestHelpers.MakeState(80, 80);
        AssignThreeToFarm(state);

        EveningPhaseSystem.ApplyPhase(state);

        // 80 + 24 = 104, zużycie 20 -> 84, psucie 4 -> 80
        Assert.AreEqual(80, state.Storehouse.Food.CurrentAmount);
        Assert.AreEqual(60, state.Storehouse.Water.CurrentAmount);
        Assert.AreEqual(24, state.EveningReport.ProducedFood);
        Assert.AreEqual(20, state.EveningReport.UsedFood);
        Assert.AreEqual(4, state.EveningReport.SpoiledFood);
    }

    [Test]
    public void ApplyPhase_FarmerProducesThirty()
    {
        var state = TestHelpers.MakeState(80, 80);
        TestHelpers.SetTrait(state, 0, "farmer");
        AssignThreeToFarm(state);

        EveningPhaseSystem.ApplyPhase(state);

        Assert.AreEqual(30, state.EveningReport.ProducedFood);
    }

    [Test]
    public void ApplyPhase_OverflowIsLostAndReported()
    {
        var state = TestHelpers.MakeState(395, 80);
        AssignThreeToFarm(state);

        EveningPhaseSystem.ApplyPhase(state);

        // 395 + 24 -> przycięte do 400 (przepadło 19), zużycie 20 -> 380, psucie 19 -> 361
        Assert.AreEqual(19, state.EveningReport.LostFood);
        Assert.AreEqual(361, state.Storehouse.Food.CurrentAmount);
    }

    [Test]
    public void ApplyPhase_FrugalResidentReducesConsumption()
    {
        var state = TestHelpers.MakeState(80, 80);
        TestHelpers.SetTrait(state, 0, "frugal");

        EveningPhaseSystem.ApplyPhase(state);

        Assert.AreEqual(19, state.EveningReport.UsedFood);
        Assert.AreEqual(19, state.EveningReport.UsedWater);
    }

    [Test]
    public void ApplyPhase_LogsProductionAndOverflow()
    {
        var state = TestHelpers.MakeState(395, 80);
        AssignThreeToFarm(state);

        EveningPhaseSystem.ApplyPhase(state);

        var texts = new System.Collections.Generic.List<string>();
        foreach (EventLogEntry entry in state.EventLog.Entries)
            texts.Add(entry.Text);

        Assert.IsTrue(texts.Exists(t => t.Contains("Wyprodukowano 24 zywnosci")));
        Assert.IsTrue(texts.Exists(t => t.Contains("przepadlo 19 zywnosci")));
    }

    [Test]
    public void EnteringMorning_ClearsAssignmentPlan()
    {
        var state = TestHelpers.MakeState();
        var advance = new AdvancePhaseCommand();

        advance.Execute(state); // Poranek -> Dzień
        new AssignPointCommand(0, "farm").Execute(state);
        advance.Execute(state); // Dzień -> Wieczór
        advance.Execute(state); // Wieczór -> Poranek

        Assert.AreEqual(Phase.Morning, state.Phase);
        Assert.AreEqual(0, state.Assignments.Entries.Count);
    }
}
