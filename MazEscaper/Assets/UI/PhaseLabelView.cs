using Core;
using TMPro;

namespace UI
{
    public class PhaseLabelView:GameView
    {
        public TextMeshPro text;
        public override void Render(GameState state)
        {
            text.text = state.Phase.ToString();
        }
    }
}