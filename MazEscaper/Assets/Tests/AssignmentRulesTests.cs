using NUnit.Framework;
using Core;

public class AssignmentRulesTests
{
    private static GameState DayState()
    {
        var state = TestHelpers.MakeState();
        state.Phase = Phase.Day;
        return state;
    }

    [Test]
    public void CanAssign_OutsideDayPhase_IsFalse()
    {
        var state = TestHelpers.MakeState(); // Poranek

        Assert.IsFalse(AssignmentRules.CanAssign(state, 0, "farm"));
    }

    [Test]
    public void CanAssign_InDayPhase_IsTrueForPositionAndRest()
    {
        var state = DayState();

        Assert.IsTrue(AssignmentRules.CanAssign(state, 0, "farm"));
        Assert.IsTrue(AssignmentRules.CanAssign(state, 0, AssignmentPlan.RestId));
    }

    [Test]
    public void CanAssign_UnknownTargetOrResident_IsFalse()
    {
        var state = DayState();

        Assert.IsFalse(AssignmentRules.CanAssign(state, 0, "nonexistent"));
        Assert.IsFalse(AssignmentRules.CanAssign(state, 99, "farm"));
    }

    [Test]
    public void FreePoints_DecreaseWithAssignments_AndBlockAtZero()
    {
        var state = DayState();
        Assert.AreEqual(3, AssignmentRules.FreePoints(state, 0));

        state.Assignments.Add(0, "farm");
        state.Assignments.Add(0, "well");
        Assert.AreEqual(1, AssignmentRules.FreePoints(state, 0));

        state.Assignments.Add(0, AssignmentPlan.RestId);
        Assert.AreEqual(0, AssignmentRules.FreePoints(state, 0));
        Assert.IsFalse(AssignmentRules.CanAssign(state, 0, "farm"));
    }

    [Test]
    public void CanAssign_PositionAtLimit_IsFalseButOtherTargetsStayOpen()
    {
        var state = DayState(); // limit stanowiska: 6 punktów
        for (int i = 0; i < 3; i++) state.Assignments.Add(0, "farm");
        for (int i = 0; i < 3; i++) state.Assignments.Add(1, "farm");

        Assert.IsFalse(AssignmentRules.CanAssign(state, 2, "farm"));
        Assert.IsTrue(AssignmentRules.CanAssign(state, 2, "well"));
        Assert.IsTrue(AssignmentRules.CanAssign(state, 2, AssignmentPlan.RestId));
    }

    [Test]
    public void CanRemove_RequiresPointsAndDayPhase()
    {
        var state = DayState();
        Assert.IsFalse(AssignmentRules.CanRemove(state, 0, "farm"));

        state.Assignments.Add(0, "farm");
        Assert.IsTrue(AssignmentRules.CanRemove(state, 0, "farm"));

        state.Phase = Phase.Evening;
        Assert.IsFalse(AssignmentRules.CanRemove(state, 0, "farm"));
    }
}
