using PlasticPipe.PlasticProtocol.Messages;

namespace Core
{
    public interface ICommand
    {
        public void Execute(GameState state);
    }
}