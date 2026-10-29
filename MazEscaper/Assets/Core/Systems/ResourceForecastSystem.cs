using System;
namespace Core
{
    public static class ResourceForecastSystem
    {
        public static ResourceForecast ForecastFood(GameState state)
        {
            int needed = ConsumptionRules.NeededFood(state);
            return Forecast(state, state.Storehouse.Food.CurrentAmount,
                amount => ConsumptionRules.FoodTomorrow(amount, needed, state.Config));
        }

        public static ResourceForecast ForecastWater(GameState state)
        {
            int needed = ConsumptionRules.NeededWater(state);
            return Forecast(state, state.Storehouse.Water.CurrentAmount,
                amount => ConsumptionRules.WaterTomorrow(amount, needed));
        }

        private static ResourceForecast Forecast(GameState state, int current, Func<int, int> nextDay)
        {
            GameConfig config = state.Config;

            int tomorrow = nextDay(current);
            int dailyBalance = tomorrow - current;
            int? daysToEmpty = DaysToEmpty(current, nextDay, config.ForecastHorizonDays);
            ForecastLevel level = GetLevel(daysToEmpty, config);

            return new ResourceForecast(tomorrow, dailyBalance, daysToEmpty, level);
        }
        private static int? DaysToEmpty(int current, Func<int, int> nextDay, int horizon)
        {
            if (current <= 0)
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