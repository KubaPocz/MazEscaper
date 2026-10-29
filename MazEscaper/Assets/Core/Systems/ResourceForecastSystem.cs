using Core.Enums;

namespace Core
{
    public static class ResourceForecastSystem
    {
        public static ResourceForecast ForecastFood(GameState state,GameConfig config)
        {
            int tomorrow = state.Storehouse.Food.CurrentAmount - ConsumptionRules.NeededFood(state)-ConsumptionRules.SpoiledFood(state.Storehouse.Food.CurrentAmount,config);
            int todayBalance = state.Storehouse.Food.CurrentAmount - tomorrow;
            int daysToEmpty = state.Storehouse.Food.CurrentAmount/(config.FoodPerResidentPerDay*state.ResidentCount);
            ForecastLevel level = daysToEmpty>config.ForecastYellowDays?ForecastLevel.Green:daysToEmpty>config.ForecastRedDays?ForecastLevel.Yellow:ForecastLevel.Red;
            
            return new ResourceForecast(tomorrow,todayBalance,daysToEmpty,level);
        }

        public static ResourceForecast ForecastWater(GameState state, GameConfig config)
        {
            int tomorrow = state.Storehouse.Water.CurrentAmount - ConsumptionRules.NeededWater(state);
            int todayBalance = state.Storehouse.Water.CurrentAmount - tomorrow;
            int daysToEmpty = state.Storehouse.Water.CurrentAmount/(config.WaterPerResidentPerDay*state.ResidentCount);
            ForecastLevel level = daysToEmpty>config.ForecastYellowDays?ForecastLevel.Green:daysToEmpty>config.ForecastRedDays?ForecastLevel.Yellow:ForecastLevel.Red;
            
            return new ResourceForecast(tomorrow,todayBalance,daysToEmpty,level);
        }
    }
}