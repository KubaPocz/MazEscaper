using NUnit.Framework;
using Core;

public class ResourceForecastSystemTests
{
    // żywność dziś, jutro, bilans, dni do zera, poziom
    [TestCase(80, 57, -23, 4, ForecastLevel.Green)]
    [TestCase(57, 36, -21, 3, ForecastLevel.Yellow)]
    [TestCase(16, 0, -16, 1, ForecastLevel.Red)]
    [TestCase(0, 0, 0, 0, ForecastLevel.Red)]
    public void ForecastFood_ReturnsExpectedValues(int food, int tomorrow, int balance, int days, ForecastLevel level)
    {
        var state = TestHelpers.MakeState(food: food);

        var forecast = ResourceForecastSystem.ForecastFood(state);

        Assert.AreEqual(tomorrow, forecast.TomorrowAmount);
        Assert.AreEqual(balance, forecast.DailyBalance);
        Assert.AreEqual(days, forecast.DaysToEmpty);
        Assert.AreEqual(level, forecast.Level);
    }

    // woda dziś, jutro, bilans, dni do zera, poziom
    [TestCase(80, 60, -20, 4, ForecastLevel.Green)]
    [TestCase(60, 40, -20, 3, ForecastLevel.Yellow)]
    [TestCase(30, 10, -20, 2, ForecastLevel.Yellow)]
    [TestCase(20, 0, -20, 1, ForecastLevel.Red)]
    [TestCase(0, 0, 0, 0, ForecastLevel.Red)]
    public void ForecastWater_ReturnsExpectedValues(int water, int tomorrow, int balance, int days, ForecastLevel level)
    {
        var state = TestHelpers.MakeState(water: water);

        var forecast = ResourceForecastSystem.ForecastWater(state);

        Assert.AreEqual(tomorrow, forecast.TomorrowAmount);
        Assert.AreEqual(balance, forecast.DailyBalance);
        Assert.AreEqual(days, forecast.DaysToEmpty);
        Assert.AreEqual(level, forecast.Level);
    }

    [Test]
    public void ForecastWater_HorizonBoundary_TenDaysIsNumberElevenIsNull()
    {
        var tenDays = ResourceForecastSystem.ForecastWater(TestHelpers.MakeState(water: 200));
        var elevenDays = ResourceForecastSystem.ForecastWater(TestHelpers.MakeState(water: 220));

        Assert.AreEqual(10, tenDays.DaysToEmpty);
        Assert.IsNull(elevenDays.DaysToEmpty);
        Assert.AreEqual(ForecastLevel.Green, elevenDays.Level);
    }

    [Test]
    public void ForecastWater_UsesWaterRate_NotFoodRate()
    {
        var state = TestHelpers.MakeState(80, 80);
        state.Config.WaterPerResidentPerDay = 3; // 5 mieszkańców * 3 = 15

        var water = ResourceForecastSystem.ForecastWater(state);
        var food = ResourceForecastSystem.ForecastFood(state);

        Assert.AreEqual(65, water.TomorrowAmount);
        Assert.AreEqual(57, food.TomorrowAmount);
    }

    // Najważniejszy test: prognoza "jutro" musi zgadzać się z prawdziwym przebiegiem gry.
    [TestCase(80, 80)]
    [TestCase(400, 400)]   // duże zapasy: psucie ma realny wpływ
    [TestCase(150, 90)]
    public void ForecastTomorrow_MatchesRealStockAfterEachDay(int food, int water)
    {
        var state = TestHelpers.MakeState(food, water);
        var command = new AdvancePhaseCommand();

        for (int day = 1; day <= 20; day++)
        {
            int predictedFood = ResourceForecastSystem.ForecastFood(state).TomorrowAmount;
            int predictedWater = ResourceForecastSystem.ForecastWater(state).TomorrowAmount;

            command.Execute(state); // Poranek -> Dzień
            command.Execute(state); // Dzień -> Wieczór (rozliczenie)
            command.Execute(state); // Wieczór -> Poranek

            Assert.AreEqual(predictedFood, state.Storehouse.Food.CurrentAmount, "żywność, dzień " + day);
            Assert.AreEqual(predictedWater, state.Storehouse.Water.CurrentAmount, "woda, dzień " + day);
        }
    }
}