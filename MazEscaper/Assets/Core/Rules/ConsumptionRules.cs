using System;

namespace Core
{
    /// <summary>
    /// Rola: reguły zużycia i psucia oraz "zapas po jednym dniu" (używane przez wieczór i prognozę).
    /// Zmienia: nic (same czyste funkcje).
    /// </summary>
    public static class ConsumptionRules
    {
        public static int NeededFood(GameState state)
        {
            return SumNeeded(state, state.Config.FoodPerResidentPerDay);
        }

        public static int NeededWater(GameState state)
        {
            return SumNeeded(state, state.Config.WaterPerResidentPerDay);
        }

        // Suma potrzeb mieszkańców; cecha (np. Skromny) zmienia potrzebę procentowo, wynik w dół.
        private static int SumNeeded(GameState state, int perResident)
        {
            int total = 0;
            foreach (Resident resident in state.Residents)
            {
                int percent = resident.Trait != null ? resident.Trait.ConsumptionPercent : 0;
                total += Math.Max(0, perResident * (100 + percent) / 100);
            }
            return total;
        }

        // Ile żywności się psuje z zapasu PO zużyciu (w dół, poniżej 20 jednostek nic).
        public static int SpoiledFood(int foodAfterUse, GameConfig config)
        {
            if (foodAfterUse <= 0)
                return 0;
            return (int)Math.Floor(foodAfterUse * (double)config.FoodSpoilageRate + 0.000001);
        }

        // Zapas po dodaniu produkcji, przycięty do pojemności (nadwyżka przepada).
        public static int StockAfterProduction(int current, int produced, int capacity)
        {
            return Math.Min(capacity, current + produced);
        }

        // Zapas żywności po jednym dniu: produkcja, potem zużycie, potem psucie reszty.
        public static int FoodAfterDay(int current, int produced, int needed, int capacity, GameConfig config)
        {
            int afterProduction = StockAfterProduction(current, produced, capacity);
            int used = Math.Min(needed, afterProduction);
            int remaining = afterProduction - used;
            return remaining - SpoiledFood(remaining, config);
        }

        // Zapas wody po jednym dniu: produkcja, potem zużycie (woda się nie psuje).
        public static int WaterAfterDay(int current, int produced, int needed, int capacity)
        {
            int afterProduction = StockAfterProduction(current, produced, capacity);
            return afterProduction - Math.Min(needed, afterProduction);
        }
    }
}