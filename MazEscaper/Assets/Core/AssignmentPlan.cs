using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Rola: plan przydziału punktów akcji na bieżący dzień (kto, gdzie, ile punktów).
    /// Same dane. Reguły (co wolno przydzielić) są w AssignmentRules.
    /// Plan jest czyszczony na początku każdego dnia (wejście w Poranek).
    /// </summary>
    public class AssignmentPlan
    {
        public const string RestId = "rest";

        private readonly List<Assignment> entries = new List<Assignment>();

        public IReadOnlyList<Assignment> Entries
        {
            get { return entries; }
        }

        public int GetPoints(int residentId, string targetId)
        {
            Assignment entry = Find(residentId, targetId);
            return entry == null ? 0 : entry.Points;
        }

        public int PointsUsedBy(int residentId)
        {
            int total = 0;
            foreach (Assignment entry in entries)
            {
                if (entry.ResidentId == residentId)
                    total += entry.Points;
            }
            return total;
        }

        public int PointsOn(string targetId)
        {
            int total = 0;
            foreach (Assignment entry in entries)
            {
                if (entry.TargetId == targetId)
                    total += entry.Points;
            }
            return total;
        }

        public void Add(int residentId, string targetId)
        {
            Assignment entry = Find(residentId, targetId);
            if (entry == null)
                entries.Add(new Assignment(residentId, targetId, 1));
            else
                entry.Points++;
        }

        public void Remove(int residentId, string targetId)
        {
            Assignment entry = Find(residentId, targetId);
            if (entry == null)
                throw new InvalidOperationException("Brak punktów do cofnięcia.");

            entry.Points--;
            if (entry.Points <= 0)
                entries.Remove(entry);
        }

        public void Clear()
        {
            entries.Clear();
        }

        private Assignment Find(int residentId, string targetId)
        {
            foreach (Assignment entry in entries)
            {
                if (entry.ResidentId == residentId && entry.TargetId == targetId)
                    return entry;
            }
            return null;
        }
    }
}