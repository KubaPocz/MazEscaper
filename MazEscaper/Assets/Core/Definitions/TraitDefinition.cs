namespace Core
{
    /// <summary>
    /// Rola: szablon cechy pasywnej (GDD 5.7). Nie zmienia się w grze.
    /// Procenty: 25 = +25%, -25 = -25%. Efekty, których jeszcze nie ma w grze, zostają zerem.
    /// Używane przez: Resident, ProductionRules, ConsumptionRules, ResidentGenerator.
    /// </summary>
    public class TraitDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public TraitDomain Domain { get; }
        public int FoodProductionPercent { get; }
        public int WaterProductionPercent { get; }
        public int ConsumptionPercent { get; }

        public TraitDefinition(string id, string name, TraitDomain domain, int foodProductionPercent = 0, int waterProductionPercent = 0,
            int consumptionPercent = 0)
        {
            Id = id;
            Name = name;
            Domain = domain;
            FoodProductionPercent = foodProductionPercent;
            WaterProductionPercent = waterProductionPercent;
            ConsumptionPercent = consumptionPercent;
        }
        
    }
}