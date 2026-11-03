using System;

namespace Core
{
    /// <summary>
    /// Rola: przydziela 1 punkt akcji osoby do celu (stanowisko albo odpoczynek).
    /// Widok sprawdza wcześniej AssignmentRules.CanAssign; niedozwolone wykonanie to błąd programisty.
    /// </summary>
    public class AssignPointCommand : IUndoableCommand
    {
        private readonly int residentId;
        private readonly string targetId;

        public AssignPointCommand(int residentId, string targetId)
        {
            this.residentId = residentId;
            this.targetId = targetId;
        }

        public void Execute(GameState state)
        {
            if (!AssignmentRules.CanAssign(state, residentId, targetId))
                throw new InvalidOperationException("Przydział niedozwolony.");
            state.Assignments.Add(residentId, targetId);
        }

        public void Undo(GameState state)
        {
            state.Assignments.Remove(residentId, targetId);
        }
    }
}