namespace Core
{
    /// <summary>
    /// Rola: szablon stanowiska produkcyjnego (GDD 4.6): co produkuje i ile punktów dziennie przyjmuje.
    /// Używane przez: GameContent, AssignmentRules, ProductionRules.
    /// </summary>
    public class PositionDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public ResourceType Resource { get; }
        public int PointLimitPerDay { get; }
        public PositionDefinition(string id, string name, ResourceType resource, int pointLimitPerDay)
        {
            Id = id;
            Name = name;
            Resource = resource;
            PointLimitPerDay = pointLimitPerDay;
        }
    }
}