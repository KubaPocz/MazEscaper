namespace Core
{
    public class AdvancePhaseCommand : ICommand
    {
        public void Execute(GameState gameState)
        {
            PhaseSystem.NextPhase(gameState);
        }
        
    }
}