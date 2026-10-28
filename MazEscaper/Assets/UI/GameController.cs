using UnityEngine;
using Core;
using Data;
using UI;

namespace MazEscaper.UI
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameConfigAsset configAsset;
        [SerializeField] private int fixedSeed = 0;

        private GameState state;

        private void Start()
        {
            UnityLogger logger = new UnityLogger();
            GameConfig config = configAsset.ToConfig();
            int seed = fixedSeed != 0
                ? fixedSeed
                : Random.Range(1, int.MaxValue);

            logger.Log($"Seed: {seed}");
            state = new GameState(config, new SeededRandom(seed));
        }
    }
}