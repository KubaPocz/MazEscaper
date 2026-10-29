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
            int foodTomorrow = currentFood - dailyUsage - (int)Math.Floor((currentFood-dailyUsage) * config.FoodSpoilageRate);
            return foodTomorrow>0?foodTomorrow:0;
        }
        public static int WaterTomorrow(int currentWater, int dailyUsage)
        {
            int waterTomorrow = currentWater - dailyUsage;
            return waterTomorrow > 0?waterTomorrow:0;
        }
    }
}