using System.Collections.Generic;
 
namespace Core
{
    // Log dla gracza (zdarzenia w grze). Nie mylić z ILogSink, który służy do diagnostyki.
    public class EventLog
    {
        private readonly List<EventLogEntry> entries = new List<EventLogEntry>();
 
        public IReadOnlyList<EventLogEntry> Entries
        {
            get { return entries; }
        }
 
        public void Add(int day, Phase phase, string text)
        {
            entries.Add(new EventLogEntry(day, phase, text));
        }
    }
}