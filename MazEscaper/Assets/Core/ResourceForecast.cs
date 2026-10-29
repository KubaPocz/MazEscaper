using Core.Enums;
namespace Core
{
    public class ResourceForecast
    {
        public int TomorrowAmount { get; }
        public int DailyBalance { get; }
        public int? DaysToEmpty { get; }
        public ForecastLevel Level { get; }

        public ResourceForecast(int tomorrowAmount, int dailyBalance, int daysToEmpty, ForecastLevel level)
        {
            TomorrowAmount = tomorrowAmount;
            DailyBalance = dailyBalance;
            DaysToEmpty = daysToEmpty;
            Level = level;
        }
    }
}