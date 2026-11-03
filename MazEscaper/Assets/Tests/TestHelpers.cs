using Core;

public static class TestHelpers
{
    // Stan z jawnie ustawionymi liczbami, żeby testy nie zależały od domyślnych wartości configu.
    // Mieszkańcy są podmieniani na 5 osób o tej samej cesze (domyślnie brak cechy),
    // więc wynik nie zależy od losowania cech.
    public static GameState MakeState(int food = 80, int water = 80, TraitDefinition trait = null)
    {
        var config = new GameConfig();
        config.StartingFood = food;
        config.StartingWater = water;
        config.StartingResidentCount = 5;
        config.ActionPointsPerDay = 3;
        config.FoodPerResidentPerDay = 4;
        config.WaterPerResidentPerDay = 4;
        config.FoodWaterPerActionPoint = 8;
        config.WoodStonePerActionPoint = 4;
        config.FoodSpoilageRate = 0.05f;
        config.FoodWaterStorageCapacity = 400;
        config.ForecastHorizonDays = 10;
        config.ForecastYellowDays = 3;
        config.ForecastRedDays = 1;

        var state = new GameState(config, new SeededRandom(1));

        state.Residents.Clear();
        for (int i = 0; i < 5; i++)
            state.Residents.Add(new Resident(i, "Test" + i, trait));

        return state;
    }

    // Cecha z treści prototypu po identyfikatorze (farmer, waterbearer, handyman, frugal, tough, tracker).
    public static TraitDefinition Trait(string id)
    {
        return GameContent.CreatePrototype().FindTrait(id);
    }

    // Podmienia jednego mieszkańca na osobę z podaną cechą.
    public static void SetTrait(GameState state, int residentId, string traitId)
    {
        state.Residents[residentId] = new Resident(residentId, "Test" + residentId, Trait(traitId));
    }
}