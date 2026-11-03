namespace Core
{
    /// <summary>
    /// Rola: wynik rozliczenia wieczoru (tylko do odczytu).
    /// Brak = potrzeba minus zużycie. Lost = nadwyżka, która przepadła przez pojemność magazynu.
    /// </summary>
    public class EveningReport
    {
        public int NeededFood { get; }
        public int UsedFood { get; }
        public int SpoiledFood { get; }
        public int NeededWater { get; }
        public int UsedWater { get; }
        public int ProducedFood { get; }
        public int ProducedWater { get; }
        public int LostFood { get; }
        public int LostWater { get; }

        public int MissingFood
        {
            get { return NeededFood - UsedFood; }
        }

        public int MissingWater
        {
            get { return NeededWater - UsedWater; }
        }

        public EveningReport(int neededFood, int usedFood, int spoiledFood, int neededWater, int usedWater)
            : this(neededFood, usedFood, spoiledFood, neededWater, usedWater, 0, 0, 0, 0)
        {
        }

        public EveningReport(int neededFood, int usedFood, int spoiledFood, int neededWater, int usedWater,
            int producedFood, int producedWater, int lostFood, int lostWater)
        {
            NeededFood = neededFood;
            UsedFood = usedFood;
            SpoiledFood = spoiledFood;
            NeededWater = neededWater;
            UsedWater = usedWater;
            ProducedFood = producedFood;
            ProducedWater = producedWater;
            LostFood = lostFood;
            LostWater = lostWater;
        }
    }
}