namespace Core
{
    /// <summary>
    /// Rola: konkretny mieszkaniec (dane zmienne w grze).
    /// Trait może być null (brak cechy, np. w testach). Experience to pole na później (GDD 5.2).
    /// </summary>
    public class Resident
    {
        public int Id { get; }
        public string Name { get; }
        public TraitDefinition Trait { get; }
        public int Experience { get; set; }

        public Resident(int id, string name, TraitDefinition trait)
        {
            Id = id;
            Name = name;
            Trait = trait;
            Experience = 0;
        }
    }
}