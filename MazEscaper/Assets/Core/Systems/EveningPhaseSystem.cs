namespace Core
{
    public static class EveningPhaseSystem
    {
        // Czysta: niczego nie zmienia w magazynie ani w logu. Może ją wołać prognoza.
        public static EveningReport CalculateReport(GameState state)
        {
            var config = state.Config;
            var food = state.Storehouse.Food;
            var water = state.Storehouse.Water;

            int neededFood = ConsumptionRules.NeededFood(state);
            int neededWater = ConsumptionRules.NeededWater(state);

            int usedFood = food.PreviewRemove(neededFood);
            int usedWater = water.PreviewRemove(neededWater);

            // Zapas po zużyciu jest wyliczony, bo w magazynie jeszcze go nie ma.
            int foodAfterUse = food.CurrentAmount - usedFood;
            int spoiledFood = ConsumptionRules.SpoiledFood(foodAfterUse, config);

            return new EveningReport(neededFood, usedFood, spoiledFood, neededWater, usedWater);
        }

        // Zmienia stan: zdejmuje zasoby według raportu, zapisuje raport i dopisuje wpisy do logu.
        public static void ApplyPhase(GameState state)
        {
            EveningReport report = CalculateReport(state);

            state.Storehouse.Food.RemoveResource(report.UsedFood);
            state.Storehouse.Food.RemoveResource(report.SpoiledFood);
            state.Storehouse.Water.RemoveResource(report.UsedWater);

            state.EveningReport = report;
            WriteLog(state, report);
        }

        private static void WriteLog(GameState state, EveningReport report)
        {
            EventLog log = state.EventLog;
            int day = state.CurrentDay;
            var phase = state.Phase;

            log.Add(day, phase, $"Zuzyto {report.UsedFood} zywnosci i {report.UsedWater} wody.");

            if (report.SpoiledFood > 0)
                log.Add(day, phase, $"Zepsulo sie {report.SpoiledFood} zywnosci.");

            int missingFood = report.NeededFood - report.UsedFood;
            if (missingFood > 0)
                log.Add(day, phase, $"Brakuje {missingFood} zywnosci.");

            int missingWater = report.NeededWater - report.UsedWater;
            if (missingWater > 0)
                log.Add(day, phase, $"Brakuje {missingWater} wody.");
        }
    }
}