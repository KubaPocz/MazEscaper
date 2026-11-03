namespace Core
{
    /// <summary>
    /// Rola: ile surowca daje obecny plan dnia (punkty na stanowiskach, premie cech).
    /// Wejście: GameState. Zmienia: nic (czysta, woła ją wieczór i prognoza).
    /// </summary>
    public static class ProductionRules
    {
        // Ile jednostek daje 1 punkt akcji (GDD 4.6). Zasoby bez produkcji dają 0.
        public static int UnitsPerPoint(GameConfig config, ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Food:
                case ResourceType.Water:
                    return config.FoodWaterPerActionPoint;
                case ResourceType.Wood:
                case ResourceType.Stone:
                    return config.WoodStonePerActionPoint;
                default:
                    return 0;
            }
        }

        public static int Produced(GameState state, ResourceType resource)
        {
            int total = 0;
            foreach (Assignment entry in state.Assignments.Entries)
            {
                PositionDefinition position = state.Content.FindPosition(entry.TargetId);
                if (position == null || position.Resource != resource)
                    continue;

                Resident resident = AssignmentRules.FindResident(state, entry.ResidentId);
                int baseUnits = entry.Points * UnitsPerPoint(state.Config, resource);
                int bonus = BonusPercent(resident, resource);
                total += baseUnits * (100 + bonus) / 100;
            }
            return total;
        }

        private static int BonusPercent(Resident resident, ResourceType resource)
        {
            if (resident == null || resident.Trait == null)
                return 0;

            switch (resource)
            {
                case ResourceType.Food: return resident.Trait.FoodProductionPercent;
                case ResourceType.Water: return resident.Trait.WaterProductionPercent;
                default: return 0;
            }
        }
    }
}