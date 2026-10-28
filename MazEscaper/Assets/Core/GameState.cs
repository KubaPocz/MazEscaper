using Core.Enums;

namespace Core
{
    public class GameState
    {
        public GameConfig Config { get; }
        public IRandom Random { get; }
        public Phase Phase { get; set; }
        public int CurrentDay { get; set; }
        public Storehouse Storehouse { get;}
        public int ResidentCount{ get; set; }

        public GameState(GameConfig config, IRandom random)
        {
            Config = config;
            Random = random;
            Phase = Phase.Morning;
            CurrentDay = 1;
            Storehouse = new Storehouse(config);
            ResidentCount = config.StartingResidentCount;
        }
    }
}
