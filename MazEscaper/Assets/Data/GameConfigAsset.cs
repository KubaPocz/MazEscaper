using UnityEngine;
using MazEscaper.Core;

namespace MazEscaper.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "MazEscaper/Game Config")]

    public class GameConfigAsset : ScriptableObject
    {
        [SerializeField] private GameConfig config = new GameConfig();

        public GameConfig ToConfig()
        {
            return config.Clone();
        }
    }
}