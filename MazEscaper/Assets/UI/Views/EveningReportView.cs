using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EveningReportView:LabelView
    {
        public override void Render(GameState state)
        {
            if (state.EveningReport is null)
            {
                text.text = $"No report from yesterday";
            }
            else
            {
                text.text = $"Needed Food: {state.EveningReport.NeededFood.ToString()}\n" +
                            $"Used Food: {state.EveningReport.UsedFood.ToString()}\n" +
                            $"Spoiled Food: {state.EveningReport.SpoiledFood.ToString()}\n" +
                            $"Needed Water: {state.EveningReport.NeededWater.ToString()}\n" +
                            $"Used Water: {state.EveningReport.UsedWater.ToString()}";
            }
        }
    }
}