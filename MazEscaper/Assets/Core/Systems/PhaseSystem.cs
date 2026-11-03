using System;

namespace Core
{
    public static class PhaseSystem
    {
        public static void NextPhase(GameState state)
        {
            if (state.Phase == Phase.Evening)
                state.CurrentDay++;
            state.Phase = (Phase)(((int)state.Phase + 1) % Enum.GetValues(typeof(Phase)).Length);
            OnPhaseEntered(state);
        }

        private static void OnPhaseEntered(GameState state)
        {
            switch (state.Phase)
            {
                case Phase.Morning:
                    state.Assignments.Clear();
                    break;
                case Phase.Day:
                    //TODO
                    break;
                case Phase.Evening:
                    EveningPhaseSystem.ApplyPhase(state);
                    break;
            }
        }
    }
}
