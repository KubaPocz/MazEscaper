using System;
namespace Core
{
    public class Storehouse
    {
        public ResourceStore Water { get; }
        public ResourceStore Food { get; }
        public ResourceStore Wood { get; }
        public ResourceStore Stone { get; }
        public ResourceStore Parts { get; }
        public ResourceStore Medicine { get; }

        public Storehouse(GameConfig config)
        {
            Water = new ResourceStore(config.FoodWaterStorageCapacity);
            Water.AddResource(config.StartingWater);
            Food = new ResourceStore(config.FoodWaterStorageCapacity);
            Food.AddResource(config.StartingFood);
            Wood = new ResourceStore(config.WoodStoneStorageCapacity);
            Wood.AddResource(config.StartingWood);
            Stone = new ResourceStore(config.WoodStoneStorageCapacity);
            Stone.AddResource(config.StartingStone);
            Parts = new ResourceStore(config.PartsStorageCapacity);
            Parts.AddResource(config.StartingParts);
            Medicine = new ResourceStore(config.MedicineStorageCapacity);
            Medicine.AddResource(config.StartingMedicine);
        }

        public ResourceStore Get(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Food:
                    return Food;
                case ResourceType.Water:
                    return Water;
                case ResourceType.Wood:
                    return Wood;
                case ResourceType.Stone:
                    return Stone;
                case ResourceType.Parts:
                    return Parts;
                case ResourceType.Medicine:
                    return Medicine;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}