using NUnit.Framework;
using Core;

public class ForecastWithProductionTests
{
    private static void AssignThree(GameState state, int residentId, string target)
    {
        for (int i = 0; i < 3; i++)
            new AssignPointCommand(residentId, target).Execute(state);
    }

    [Test]
    public void Forecast_UpdatesLiveWhenPointsAreAssigned()
    {
        var state = TestHelpers.MakeState(80, 80);
        state.Phase = Phase.Day;

        Assert.AreEqual(57, ResourceForecastSystem.ForecastFood(state).TomorrowAmount);

        AssignThree(state, 0, "farm");

        // 80 + 24 - 20 = 84, psucie 4 -> 80
        Assert.AreEqual(80, ResourceForecastSystem.ForecastFood(state).TomorrowAmount);
    }

    [Test]
    public void Forecast_ProductionAboveConsumption_NeverRunsOut()
    {
        var state = TestHelpers.MakeState(80, 80);
        state.Phase = Phase.Day;
        AssignThree(state, 0, "farm");
        AssignThree(state, 1, "farm"); // 48 > 20 zużycia

        var forecast = ResourceForecastSystem.ForecastFood(state);

        Assert.IsNull(forecast.DaysToEmpty);
        Assert.AreEqual(ForecastLevel.Green, forecast.Level);
    }

    // Najważniejszy test: prognoza "jutro" zgadza się z prawdziwym wieczorem także przy produkcji i pełnym magazynie.
    [TestCase(80, 80)]
    [TestCase(380, 380)]  // magazyn pełni się i przycina nadwyżkę
    [TestCase(150, 90)]
    public void ForecastTomorrow_MatchesRealStock_WithAssignmentsEveryDay(int food, int water)
    {
        var state = TestHelpers.MakeState(food, water);
        var advance = new AdvancePhaseCommand();

        for (int day = 1; day <= 20; day++)
        {
            advance.Execute(state); // Poranek -> Dzień
            AssignThree(state, 0, "farm");
            AssignThree(state, 1, "well");

            int predictedFood = ResourceForecastSystem.ForecastFood(state).TomorrowAmount;
            int predictedWater = ResourceForecastSystem.ForecastWater(state).TomorrowAmount;

            advance.Execute(state); // Dzień -> Wieczór (rozliczenie)

            Assert.AreEqual(predictedFood, state.Storehouse.Food.CurrentAmount, "żywność, dzień " + day);
            Assert.AreEqual(predictedWater, state.Storehouse.Water.CurrentAmount, "woda, dzień " + day);

            advance.Execute(state); // Wieczór -> Poranek
        }
    }
}
