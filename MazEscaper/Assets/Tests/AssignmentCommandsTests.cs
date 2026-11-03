using System;
using NUnit.Framework;
using Core;

public class AssignmentCommandsTests
{
    private static GameState DayState()
    {
        var state = TestHelpers.MakeState();
        state.Phase = Phase.Day;
        return state;
    }

    [Test]
    public void AssignPoint_ExecuteAddsPoint_UndoRemovesIt()
    {
        var state = DayState();
        var command = new AssignPointCommand(0, "farm");

        command.Execute(state);
        Assert.AreEqual(1, state.Assignments.GetPoints(0, "farm"));

        command.Undo(state);
        Assert.AreEqual(0, state.Assignments.GetPoints(0, "farm"));
    }

    [Test]
    public void RemovePoint_ExecuteRemovesPoint_UndoRestoresIt()
    {
        var state = DayState();
        new AssignPointCommand(0, "farm").Execute(state);
        var remove = new RemovePointCommand(0, "farm");

        remove.Execute(state);
        Assert.AreEqual(0, state.Assignments.GetPoints(0, "farm"));

        remove.Undo(state);
        Assert.AreEqual(1, state.Assignments.GetPoints(0, "farm"));
    }

    [Test]
    public void AssignPoint_WhenNotAllowed_Throws()
    {
        var state = TestHelpers.MakeState(); // Poranek, przydział niedozwolony

        Assert.Throws<InvalidOperationException>(() => new AssignPointCommand(0, "farm").Execute(state));
    }

    [Test]
    public void RemovePoint_WithoutPoints_Throws()
    {
        var state = DayState();

        Assert.Throws<InvalidOperationException>(() => new RemovePointCommand(0, "farm").Execute(state));
    }

    [Test]
    public void UndoInReverseOrder_RestoresPreviousPlan()
    {
        var state = DayState();
        var first = new AssignPointCommand(0, "farm");
        var second = new AssignPointCommand(1, "well");
        first.Execute(state);
        second.Execute(state);

        second.Undo(state);
        first.Undo(state);

        Assert.AreEqual(0, state.Assignments.Entries.Count);
    }
}
