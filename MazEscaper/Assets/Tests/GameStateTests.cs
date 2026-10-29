using NUnit.Framework;
using Core;

namespace Tests
{
    public class GameStateTests
    {
        [Test]
        public void NewState_HasDayOne()
        {
            var config = new GameConfig();
            var random = new SeededRandom(42);
            var gameState = new GameState(config, random);
            Assert.AreEqual(1, gameState.CurrentDay);
        }

        [Test]
        public void NewState_StoreHouseHasCorrectResourceAmount()
        {
            var config = new GameConfig();
            var random = new SeededRandom(42);
            var gameState = new GameState(config, random);
            Assert.AreEqual(config.StartingWater, gameState.Storehouse.Water.CurrentAmount);
            Assert.AreEqual(config.StartingFood, gameState.Storehouse.Food.CurrentAmount);
            Assert.AreEqual(config.StartingWood, gameState.Storehouse.Wood.CurrentAmount);
            Assert.AreEqual(config.StartingStone, gameState.Storehouse.Stone.CurrentAmount);
            Assert.AreEqual(config.StartingParts, gameState.Storehouse.Parts.CurrentAmount);
            Assert.AreEqual(config.StartingMedicine, gameState.Storehouse.Medicine.CurrentAmount);
        }

        [Test]
        public void StatesAreIndependentBetweenInstances()
        {
            var config = new GameConfig();
            var random = new SeededRandom(42);
            var gameState1 = new GameState(config, random);
            var gameState2 = new GameState(config, random);
            gameState1.Storehouse.Food.AddResource(20);
            Assert.AreEqual(gameState1.Storehouse.Food.CurrentAmount, config.StartingFood + 20);
            Assert.AreEqual(gameState2.Storehouse.Food.CurrentAmount, config.StartingFood);
        }
    }
}