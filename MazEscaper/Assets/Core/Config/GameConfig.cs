using System;

[Serializable]
public class GameConfig
{
    // ---------- Część 0: setup ----------
    public int ActionPointsPerDay = 3; // GDD 4.3
    public int MazeSize = 5; // GDD 13.2 (5x5)
    public int MissionTimeBudget = 10; // GDD 3.5
    public float FoodSpoilageRate = 0.05f; // GDD 4.7 (zaokrąglane w dół w kodzie)

    // ---------- Część 1: dzień i surowce ----------
    public int FoodPerResidentPerDay = 1; // GDD 4.5
    public int WaterPerResidentPerDay = 1; // GDD 4.5
    public int FoodWaterStorageCapacity = 100; // ROBOCZE
    public int WoodStoneStorageCapacity = 100; // ROBOCZE
    public int PartsStorageCapacity = 100; // ROBOCZE
    public int MedicineStorageCapacity = 100; // ROBOCZE
    public int StartingFood = 20; // ROBOCZE
    public int StartingWater = 20; // ROBOCZE
    public int StartingWood = 10; // ROBOCZE
    public int StartingStone = 5; // ROBOCZE
    public int StartingParts = 0; // ROBOCZE
    public int StartingMedicine = 0; // ROBOCZE
    public int ForecastYellowDays = 3; // ROBOCZE: próg żółtego koloru prognozy
    public int ForecastRedDays = 1; // ROBOCZE: próg żółtego koloru prognozy
    public int ForecastHorizonDays = 10; // ROBOCZE: próg żółtego koloru prognozy

    // ---------- Część 2: mieszkańcy i PA ----------
    public int StartingResidentCount = 5; // plan, część 2 (GDD: 4-6)
    public int FoodWaterPerActionPoint = 2; // GDD 4.6
    public int WoodStonePerActionPoint = 1; // GDD 4.6
    public int RestCostPoints = 1; // GDD 4.6
    public int MinTraitDomainsAtStart = 3; // GDD 5.7
    public int StartingMorale = 3; // GDD 5.9
    public int MinMorale = 0; // GDD 5.9
    public int MaxMorale = 5; // GDD 5.9

    // ---------- Część 3: statusy i wydarzenia w osadzie ----------
    public float RiskChancePerPoint = 0.05f; // GDD 5.8 (uraz nogi, oparzenie)
    public int MaxNegativeStatuses = 2; // GDD 5.8
    public int MaxTotalApPenalty = 2; // GDD 5.8
    public int MinApForAbleResident = 1; // GDD 5.8
    public int ProtectionPeriodDays = 1; // GDD 5.8
    public int FatigueWorkDaysThreshold = 3; // GDD 5.8
    public int ExhaustionDaysThreshold = 2; // GDD 5.8
    public int HungerToInjuredDays = 2; // GDD 5.8
    public float DailyEventChance = 0.30f; // GDD 4.9
    public int MaxSettlementEventsPerDay = 1; // GDD 4.9
    public int CampfireApCost = 1; // GDD 4.6
    public int RepairCostPoints = 2; // GDD 4.6

    // ---------- Część 4: ekspedycja ----------
    public int MinCrewSize = 2; // GDD 3.3
    public int MaxCrewSize = 4; // GDD 3.3
    public int ExpeditionFoodPerPersonPerDay = 1; // GDD 3.4
    public int ExpeditionWaterPerPersonPerDay = 1; // GDD 3.4
    public int SearchLocationTimeCost = 2; // GDD 3.5
    public int ExpeditionDurationWithNightDays = 2; // GDD 2.2

    // ---------- Część 5: zdarzenia w wyprawie i śmierć ----------
    public int MinRandomEventsPerExpedition = 1; // GDD 7.4
    public int MaxRandomEventsPerExpedition = 3; // GDD 7.4
    public int CareCostPoints = 1; // GDD 4.6
    public int UntreatedDeathDays = 3; // GDD 5.8 (Ciężko ranny)
    public int MoraleLossOnCompanionDeath = 1; // GDD 5.9 (wersja bez przyjaźni)
    public float ChanceLevelLowMax = 0.35f; // ROBOCZE: do tego progu "niska"
    public float ChanceLevelHighMin = 0.65f; // ROBOCZE: od tego progu "wysoka"

    // ---------- Część 6: zamknięcie pętli ----------
    public int ClosingCounterDays = 25; // plan, część 6 (GDD: 40-25)
    public int EscapeElementsRequired = 1; // plan, część 6

    public GameConfig Clone()
    {
        return (GameConfig)MemberwiseClone();
    }
}