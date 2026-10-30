namespace Core
{
    public class EventLogEntry
    {
        public int Day { get; }
        public Phase Phase { get; }
        public string Text { get; }
 
        public EventLogEntry(int day, Phase phase, string text)
        {
            Day = day;
            Phase = phase;
            Text = text;
        }
    }
}