using Core;
using NUnit.Framework;
namespace Tests
{
    public class PhaseSystemTests
    {
        [Test]
        public void ExecutingAdvancePhaseCommand_AtMorning_ChangePhase()
        {
            var config = new GameConfig();
            var random = new SeededRandom(42);
            GameState state = new GameState(config, random);
            AdvancePhaseCommand command =  new AdvancePhaseCommand();
            command.Execute(state);
            Assert.AreEqual(Phase.Day, state.Phase);
            Assert.AreEqual(1, state.CurrentDay);
        }
        [Test]
        public void ExecutingAdvancePhaseCommand_AtDay_ChangePhase()
        {
            var config = new GameConfig();
            var random = new SeededRandom(42);
            GameState state = new GameState(config, random);
            AdvancePhaseCommand command =  new AdvancePhaseCommand();
            command.Execute(state);
            command.Execute(state);
            Assert.AreEqual(Phase.Evening, state.Phase);
            Assert.AreEqual(1, state.CurrentDay);
        }
        [Test]
        public void ExecutingAdvancePhaseCommand_AtEvening_ChangePhaseAndDay()
        {
            var config = new GameConfig();
            var random = new SeededRandom(42);
            GameState state = new GameState(config, random);
            AdvancePhaseCommand command =  new AdvancePhaseCommand();
            command.Execute(state);
            command.Execute(state);
            Assert.AreEqual(1, state.CurrentDay);
            Assert.AreEqual(Phase.Evening, state.Phase);
            command.Execute(state);
            Assert.AreEqual(2, state.CurrentDay);
            Assert.AreEqual(Phase.Morning, state.Phase);
        }
        [Test]
        public void ExecutingAdvancePhaseCommand_3Times_ChangeDay()
        {
            var config = new GameConfig();
            var random = new SeededRandom(42);
            GameState state = new GameState(config, random);
            AdvancePhaseCommand command =  new AdvancePhaseCommand();
            command.Execute(state);
            command.Execute(state);
            command.Execute(state);
            Assert.AreEqual(2, state.CurrentDay);
            Assert.AreEqual(Phase.Morning, state.Phase);
        }
        [Test]
        public void ExecutingAdvancePhaseCommand_30Times_ChangeDayTo11AndPhaseToMorning()
        {
            var config = new GameConfig();
            var random = new SeededRandom(42);
            GameState state = new GameState(config, random);
            AdvancePhaseCommand command =  new AdvancePhaseCommand();
            for (int i = 1; i <= 30; i++)
                command.Execute(state);
            Assert.AreEqual(11, state.CurrentDay);
            Assert.AreEqual(Phase.Morning, state.Phase);
        }
    }
}