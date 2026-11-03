namespace Core
{
    // Komenda, którą da się cofnąć. Cofanie wołamy w odwrotnej kolejności niż wykonanie (stos w kontrolerze).
    public interface IUndoableCommand : ICommand
    {
        void Undo(GameState state);
    }
}