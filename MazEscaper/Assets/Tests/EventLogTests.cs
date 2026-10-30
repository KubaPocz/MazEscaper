using System.Linq;
using NUnit.Framework;
using Core;

namespace Tests
{
    public class EventLogTests
    {
        [Test]
        public void ApplyPhase_LogsUseAndSpoilage_NoShortage()
        {
            var state = TestHelpers.MakeState(80, 80);

            EveningPhaseSystem.ApplyPhase(state);

            var texts = state.EventLog.Entries.Select(e => e.Text).ToList();
            Assert.IsTrue(texts.Any(t => t.Contains("Zuzyto 20 zywnosci i 20 wody")));
            Assert.IsTrue(texts.Any(t => t.Contains("Zepsulo sie 3 zywnosci")));
            Assert.IsFalse(texts.Any(t => t.Contains("Brakuje")));
        }

        [Test]
        public void ApplyPhase_NoSpoilageEntry_WhenNothingSpoiled()
        {
            var state = TestHelpers.MakeState(food: 25);

            EveningPhaseSystem.ApplyPhase(state);

            Assert.IsFalse(state.EventLog.Entries.Any(e => e.Text.Contains("Zepsulo")));
        }

        [Test]
        public void ApplyPhase_LogsShortage()
        {
            var state = TestHelpers.MakeState(food: 16, water: 10);

            EveningPhaseSystem.ApplyPhase(state);

            var texts = state.EventLog.Entries.Select(e => e.Text).ToList();
            Assert.IsTrue(texts.Any(t => t.Contains("Brakuje 4 zywnosci")));
            Assert.IsTrue(texts.Any(t => t.Contains("Brakuje 10 wody")));
        }

        [Test]
        public void CalculateReport_DoesNotWriteToLog()
        {
            var state = TestHelpers.MakeState();

            EveningPhaseSystem.CalculateReport(state);

            Assert.AreEqual(0, state.EventLog.Entries.Count);
        }
    }
}