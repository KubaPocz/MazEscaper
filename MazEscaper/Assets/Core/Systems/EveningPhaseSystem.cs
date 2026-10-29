namespace Core
{
    public static class EveningPhaseSystem
    {
        public static EveningReport CalculateReport(GameState state)
        {
            var config = state.Config;
            var food = state.Storehouse.Food;
            var water = state.Storehouse.Water;
 
            int neededFood = ConsumptionRules.NeededFood(state);
            int neededWater = ConsumptionRules.NeededWater(state);
            
            int usedFood = food.PreviewRemove(neededFood);
            int usedWater = water.PreviewRemove(neededWater);
 
            int foodAfterUse = food.CurrentAmount - usedFood;
            int spoiledFood = ConsumptionRules.SpoiledFood(foodAfterUse, config);
 
            return new EveningReport(neededFood, usedFood, spoiledFood, neededWater, usedWater);
        }

        public static void ApplyPhase(GameState state)
        {
            EveningReport report = CalculateReport(state);
 
            state.Storehouse.Food.RemoveResource(report.UsedFood);
            state.Storehouse.Food.RemoveResource(report.SpoiledFood);
            state.Storehouse.Water.RemoveResource(report.UsedWater);
 
            state.EveningReport = report;
            
        }
    }
}