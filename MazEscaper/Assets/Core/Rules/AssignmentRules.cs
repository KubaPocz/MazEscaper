using System;

namespace Core
{
    /// <summary>
    /// Rola: reguły przydziału punktów akcji (co wolno przydzielić i cofnąć).
    /// Wejście: GameState. Zmienia: nic (czyste funkcje, może je wołać widok).
    /// Używane przez: komendy przydziału, widoki.
    /// </summary>
    public static class AssignmentRules
    {
        public static Resident FindResident(GameState state, int residentId)
        {
            foreach (Resident resident in state.Residents)
            {
                if (resident.Id == residentId)
                    return resident;
            }
            return null;
        }

        //returns how many PA have this resident per day
        //TODO: part 3 will substract PA here
        public static int AvailablePoints(GameState state, Resident resident)
        {
            return state.Config.ActionPointsPerDay;
        }

        //returns free points of resident with this id
        public static int FreePoints(GameState state, int residentId)
        {
            Resident resident = FindResident(state, residentId);
            if (resident == null)
                return 0;
            int free = AvailablePoints(state, resident) - state.Assignments.PointsUsedBy(residentId);
            return Math.Max(0, free);
        }

        public static bool CanAssign(GameState state, int residentId, string targetId)
        {
            if (state.Phase != Phase.Day)
                return false;
            if (FindResident(state, residentId) == null)
                return false;
            if (FreePoints(state, residentId) <= 0)
                return false;

            if (targetId == AssignmentPlan.RestId)
                return true;

            PositionDefinition position = state.Content.FindPosition(targetId);
            if (position == null)
                return false;

            return state.Assignments.PointsOn(targetId) < position.PointLimitPerDay;
        }

        public static bool CanRemove(GameState state, int residentId, string targetId)
        {
            return state.Phase == Phase.Day && state.Assignments.GetPoints(residentId, targetId) > 0;
        }
    }
}