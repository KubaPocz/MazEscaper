using Core;
using TMPro;

namespace UI
{
    public class PhaseLabelView:LabelView
    {
        public override void Render(GameState state)
        {
            text.text = $"Phase: {state.Phase.ToString()}";
        }
    }
}