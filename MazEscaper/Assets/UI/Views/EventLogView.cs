using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Core;

namespace UI
{
// Pokazuje cały log zdarzeń w jednym polu tekstowym wewnątrz Scroll View.
    public class EventLogView : GameView
    {
        [SerializeField] private TMP_Text logText;
        [SerializeField] private ScrollRect scrollRect;

        private readonly StringBuilder builder = new StringBuilder();

        public override void Render(GameState state)
        {
            builder.Clear();
            foreach (EventLogEntry entry in state.EventLog.Entries)
                builder.AppendLine($"Dzien {entry.Day}, {PhaseName(entry.Phase)}: {entry.Text}");

            logText.text = builder.ToString();

            // Przewiń na dół, żeby najnowsze wpisy były widoczne.
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        // Bez polskich znaków, dopóki czcionka TMP nie ma ogonków.
        private static string PhaseName(Phase phase)
        {
            switch (phase)
            {
                case Phase.Morning: return "Poranek";
                case Phase.Day: return "Dzien";
                case Phase.Evening: return "Wieczor";
                default: return phase.ToString();
            }
        }
    }
}