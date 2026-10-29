using UnityEngine;
using Core;
namespace UI
{
    public abstract class GameView :MonoBehaviour
    {
        public abstract void Render(GameState state);
    }
}
