using System.Collections.Generic;

namespace Core
{
    public class GameState
    {
        public GameConfig Config { get; }
        public IRandom Random { get; }
        public Phase Phase { get; set; }
        public int CurrentDay { get; set; }
        public Storehouse Storehouse { get;}
        public EveningReport EveningReport { get; set; }
        public EventLog EventLog { get; }
        public GameContent Content { get; }
        public List<Resident> Residents { get; }
        public AssignmentPlan Assignments { get; } = new AssignmentPlan();
        public int ResidentCount { get { return Residents.Count; } }

        public GameState(GameConfig config, IRandom random, GameContent content =null)
        {
            Config = config;
            Random = random;
            Phase = Phase.Morning;
            CurrentDay = 1;
            Storehouse = new Storehouse(config);
            EveningReport = null;
            EventLog = new EventLog();
            Content = content ?? GameContent.CreatePrototype();
            Residents = ResidentGeneratorSystem.Generate(config.StartingResidentCount, Content, random);
        }
    }
}
