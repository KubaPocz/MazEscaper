namespace Core
{
    public class AdvancePhaseCommand : ICommand
    {
        private readonly PhaseSystem phaseSystem = new PhaseSystem();
        public void Execute(GameState gameState)
        {
            phaseSystem.NextPhase(gameState);
        }
        
    }
}