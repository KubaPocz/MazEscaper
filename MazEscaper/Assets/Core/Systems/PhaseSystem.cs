using System;
using Core.Enums;

namespace Core
{
    public class PhaseSystem
    {
        public void NextPhase(GameState state)
        {
            if (state.Phase == Phase.Evening)
                state.CurrentDay++;
            state.Phase = (Phase)(((int)state.Phase + 1) % Enum.GetValues(typeof(Phase)).Length);
        }
    }
}
