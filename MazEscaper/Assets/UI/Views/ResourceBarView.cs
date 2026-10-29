using Core;
using UnityEngine;

namespace UI
{
    public class ResourceBarView : GameView
    {
        [SerializeField] private ResourceRowView[] rows;
 
        public override void Render(GameState state)
        {
            foreach (ResourceRowView row in rows)
                row.Render(state);
        }
    }
}