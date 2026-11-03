using System;

namespace Core
{
    /// <summary>
    /// Rola: rozliczenie wieczoru: produkcja z planu dnia, zużycie, psucie, wpisy do logu.
    /// Kolejność (GDD 2.1, 4.7): produkcja, potem zużycie, potem psucie pozostałej żywności.
    /// Wejście: GameState. Zmienia: Storehouse, EveningReport, EventLog (tylko ApplyPhase).
    /// Używane przez: PhaseSystem.
    /// </summary>
    public static class EveningPhaseSystem
    {
        // Czysta: niczego nie zmienia w magazynie ani w logu. Może ją wołać prognoza.
        public static EveningReport CalculateReport(GameState state)
        {
            GameConfig config = state.Config;
            ResourceStore food = state.Storehouse.Food;
            ResourceStore water = state.Storehouse.Water;

            int producedFood = ProductionRules.Produced(state, ResourceType.Food);
            int producedWater = ProductionRules.Produced(state, ResourceType.Water);
            int neededFood = ConsumptionRules.NeededFood(state);
            int neededWater = ConsumptionRules.NeededWater(state);

            int foodAfterProduction = ConsumptionRules.StockAfterProduction(food.CurrentAmount, producedFood, food.Capacity);
            int waterAfterProduction = ConsumptionRules.StockAfterProduction(water.CurrentAmount, producedWater, water.Capacity);

            int lostFood = food.CurrentAmount + producedFood - foodAfterProduction;
            int lostWater = water.CurrentAmount + producedWater - waterAfterProduction;

            int usedFood = Math.Min(neededFood, foodAfterProduction);
            int usedWater = Math.Min(neededWater, waterAfterProduction);

            int spoiledFood = ConsumptionRules.SpoiledFood(foodAfterProduction - usedFood, config);

            return new EveningReport(neededFood, usedFood, spoiledFood, neededWater, usedWater,
                producedFood, producedWater, lostFood, lostWater);
        }

        // Zmienia stan: dodaje produkcję, zdejmuje zużycie i psucie, zapisuje raport, dopisuje wpisy do logu.
        // Produkcja dotyczy na razie tylko żywności i wody (jedyne stanowiska w prototypie).
        public static void ApplyPhase(GameState state)
        {
            EveningReport report = CalculateReport(state);

            state.Storehouse.Food.AddResource(report.ProducedFood);
            state.Storehouse.Water.AddResource(report.ProducedWater);

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
            Phase phase = state.Phase;

            if (report.ProducedFood > 0 || report.ProducedWater > 0)
                log.Add(day, phase, $"Wyprodukowano {report.ProducedFood} zywnosci i {report.ProducedWater} wody.");

            if (report.LostFood > 0)
                log.Add(day, phase, $"Magazyn pelny: przepadlo {report.LostFood} zywnosci.");
            if (report.LostWater > 0)
                log.Add(day, phase, $"Magazyn pelny: przepadlo {report.LostWater} wody.");

            log.Add(day, phase, $"Zuzyto {report.UsedFood} zywnosci i {report.UsedWater} wody.");

            if (report.SpoiledFood > 0)
                log.Add(day, phase, $"Zepsulo sie {report.SpoiledFood} zywnosci.");

            if (report.MissingFood > 0)
                log.Add(day, phase, $"Brakuje {report.MissingFood} zywnosci.");
            if (report.MissingWater > 0)
                log.Add(day, phase, $"Brakuje {report.MissingWater} wody.");
        }
    }
}