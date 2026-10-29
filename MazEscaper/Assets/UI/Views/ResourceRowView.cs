using TMPro;
using UnityEngine;
using Core;

namespace UI
{
    public class ResourceRowView : GameView
    {
        [SerializeField] private ResourceType resourceType;

        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private TMP_Text tomorrowText;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private TMP_Text daysText;

        [SerializeField] private Color greenColor = new Color(0.30f, 0.80f, 0.30f);
        [SerializeField] private Color yellowColor = new Color(0.95f, 0.80f, 0.20f);
        [SerializeField] private Color redColor = new Color(0.90f, 0.25f, 0.25f);
        [SerializeField] private Color neutralColor = Color.white;

        public override void Render(GameState state)
        {
            ResourceStore store = state.Storehouse.Get(resourceType);

            nameText.text = GetName(resourceType);
            amountText.text = $"{store.CurrentAmount} / {store.Capacity}";

            ResourceForecast forecast = GetForecast(state);
            bool hasForecast = forecast != null;

            tomorrowText.gameObject.SetActive(hasForecast);
            balanceText.gameObject.SetActive(hasForecast);
            daysText.gameObject.SetActive(hasForecast);

            if (!hasForecast)
            {
                amountText.color = neutralColor;
                return;
            }

            tomorrowText.text = $"jutro: {forecast.TomorrowAmount}";
            balanceText.text = forecast.DailyBalance.ToString("+0;-0;0");
            daysText.text = FormatDays(forecast.DaysToEmpty, state.Config.ForecastHorizonDays);
            amountText.color = GetColor(forecast.Level);
        }

        private ResourceForecast GetForecast(GameState state)
        {
            switch (resourceType)
            {
                case ResourceType.Food: return ResourceForecastSystem.ForecastFood(state);
                case ResourceType.Water: return ResourceForecastSystem.ForecastWater(state);
                default: return null; // pozostałe zasoby nie mają zużycia, więc nie mają prognozy
            }
        }

        private static string FormatDays(int? days, int horizon)
        {
            if (days == null)
                return $"ponad {horizon} dni";
            return days == 1 ? "1 dzien" : $"{days} dni";
        }

        private Color GetColor(ForecastLevel level)
        {
            switch (level)
            {
                case ForecastLevel.Green: return greenColor;
                case ForecastLevel.Yellow: return yellowColor;
                default: return redColor;
            }
        }

        // Nazwy bez polskich znaków, dopóki czcionka TMP nie ma ogonków.
        private static string GetName(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Food: return "Zywnosc";
                case ResourceType.Water: return "Woda";
                case ResourceType.Wood: return "Drewno";
                case ResourceType.Stone: return "Kamien";
                case ResourceType.Parts: return "Czesci";
                case ResourceType.Medicine: return "Leki";
                default: return type.ToString();
            }
        }
    }
}