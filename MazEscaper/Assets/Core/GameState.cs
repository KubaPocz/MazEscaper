using MazEscaper.Core.Enums;

namespace MazEscaper.Core
{
    public class GameState
    {
        public GameConfig Config { get; }
        public IRandom Random { get; }
        public Phase Phase { get; set; }

        public GameState(GameConfig config, IRandom random)
        {
            Config = config;
            Random = random;
            Phase = Phase.Morning;
        }
    }
}
