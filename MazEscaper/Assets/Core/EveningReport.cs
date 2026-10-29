namespace Core
{
    public class EveningReport
    {
        public int NeededFood { get; }
        public int UsedFood { get;}
        public int SpoiledFood { get;}
        public int NeededWater { get;}
        public int UsedWater { get;}
        public EveningReport(int neededFood, int usedFood, int spoiledFood, int neededWater, int usedWater)
        {
            NeededFood = neededFood;
            UsedFood = usedFood;
            SpoiledFood = spoiledFood;
            NeededWater = neededWater;
            UsedWater = usedWater;
        }
    }
}