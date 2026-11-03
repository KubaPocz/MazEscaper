using System;

namespace Core
{
    /// <summary>
    /// Rola: cofa 1 punkt akcji osoby z celu (przycisk "-" przy stanowisku).
    /// </summary>
    public class RemovePointCommand : IUndoableCommand
    {
        private readonly int residentId;
        private readonly string targetId;

        public RemovePointCommand(int residentId, string targetId)
        {
            this.residentId = residentId;
            this.targetId = targetId;
        }

        public void Execute(GameState state)
        {
            if (!AssignmentRules.CanRemove(state, residentId, targetId))
                throw new InvalidOperationException("Brak punktu do cofnięcia.");
            state.Assignments.Remove(residentId, targetId);
        }

        public void Undo(GameState state)
        {
            state.Assignments.Add(residentId, targetId);
        }
    }
}