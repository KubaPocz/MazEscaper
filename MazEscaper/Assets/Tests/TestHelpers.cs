using Core;

public static class TestHelpers
{
    public static GameState MakeState(int food = 80, int water = 80)
    {
        var config = new GameConfig();
        config.StartingFood = food;
        config.StartingWater = water;
        config.StartingResidentCount = 5;
        config.FoodPerResidentPerDay = 4;
        config.WaterPerResidentPerDay = 4;
        config.FoodWaterStorageCapacity = 400;
        config.FoodSpoilageRate = 0.05f;
        config.ForecastHorizonDays = 10;
        config.ForecastYellowDays = 3;
        config.ForecastRedDays = 1;
        return new GameState(config, new SeededRandom(1));
    }
}