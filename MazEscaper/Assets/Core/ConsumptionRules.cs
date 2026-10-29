using System;

namespace Core
{
    public static class ConsumptionRules
    {
        public static int NeededFood(GameState state)
        {
            return state.Config.FoodPerResidentPerDay*state.ResidentCount;
        }
        public static int NeededWater(GameState state)
        {
            return state.Config.WaterPerResidentPerDay*state.ResidentCount;
        }

        public static int SpoiledFood(int currentFood, GameConfig config)
        {
            return (int)Math.Floor(currentFood * config.FoodSpoilageRate);
        }
        public static int FoodTomorrow(int currentFood, int dailyUsage, GameConfig config)
        {
            return currentFood-dailyUsage-(int)Math.Floor((currentFood-dailyUsage)>0?(currentFood-dailyUsage):0 * config.FoodSpoilageRate);
        }
        public static int WaterTomorrow(int currentWater, int dailyUsage)
        {
            return currentWater-dailyUsage;
        }
    }
}