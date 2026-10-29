using Core;

namespace UI
{
    public class DayLabelView:LabelView
    {
        public override void Render(GameState state)
        {
            text.text = $"Day: {state.CurrentDay.ToString()}";
        }
    }
}