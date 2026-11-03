using System;

namespace Core
{
    /// <summary>
    /// Rola: prognoza żywności i wody (jutro, bilans, dni do wyczerpania, poziom).
    /// Zakłada, że obecny plan dnia (przydział punktów) powtarza się w kolejnych dniach.
    /// Zmienia: nic (czysta, liczy tymi samymi funkcjami co wieczór).
    /// </summary>
    public static class ResourceForecastSystem
    {
        public static ResourceForecast ForecastFood(GameState state)
        {
            int needed = ConsumptionRules.NeededFood(state);
            int produced = ProductionRules.Produced(state, ResourceType.Food);
            int capacity = state.Storehouse.Food.Capacity;
            return Forecast(state, state.Storehouse.Food.CurrentAmount,
                amount => ConsumptionRules.FoodAfterDay(amount, produced, needed, capacity, state.Config));
        }

        public static ResourceForecast ForecastWater(GameState state)
        {
            int needed = ConsumptionRules.NeededWater(state);
            int produced = ProductionRules.Produced(state, ResourceType.Water);
            int capacity = state.Storehouse.Water.Capacity;
            return Forecast(state, state.Storehouse.Water.CurrentAmount,
                amount => ConsumptionRules.WaterAfterDay(amount, produced, needed, capacity));
        }

        // Wspólna część: nextDay to funkcja "zapas dziś -> zapas jutro" dla danego zasobu.
        private static ResourceForecast Forecast(GameState state, int current, Func<int, int> nextDay)
        {
            GameConfig config = state.Config;

            int tomorrow = nextDay(current);
            int dailyBalance = tomorrow - current;
            int? daysToEmpty = DaysToEmpty(current, nextDay, config.ForecastHorizonDays);
            ForecastLevel level = GetLevel(daysToEmpty, config);

            return new ResourceForecast(tomorrow, dailyBalance, daysToEmpty, level);
        }

        // Liczba dni do zera (0..horizon) albo null, gdy zapas starcza dłużej niż horyzont.
        private static int? DaysToEmpty(int current, Func<int, int> nextDay, int horizon)
        {
            // Zapas pusty dziś i pusty jutro: wyczerpany już teraz.
            if (current <= 0 && nextDay(current) <= 0)
                return 0;

            for (int day = 1; day <= horizon; day++)
            {
                current = nextDay(current);
                if (current <= 0)
                    return day;
            }

            return null;
        }

        private static ForecastLevel GetLevel(int? daysToEmpty, GameConfig config)
        {
            if (daysToEmpty == null)
                return ForecastLevel.Green;
            if (daysToEmpty > config.ForecastYellowDays)
                return ForecastLevel.Green;
            if (daysToEmpty > config.ForecastRedDays)
                return ForecastLevel.Yellow;
            return ForecastLevel.Red;
        }
    }
}