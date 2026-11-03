using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Rola: treść gry (cechy, stanowiska, imiona), przekazywana do GameState.
    /// Na razie budowana w kodzie (CreatePrototype). Gdy treści będzie dużo, ta sama struktura
    /// zostanie wypełniana z ScriptableObjectów w Data/, bez zmian w systemach.
    /// </summary>
    public class GameContent
    {
        public IReadOnlyList<TraitDefinition> Traits { get;}
        public IReadOnlyList<PositionDefinition> Positions { get;}
        public IReadOnlyList<string> ResidentNames { get;}

        public GameContent(IReadOnlyList<TraitDefinition> traits,
            IReadOnlyList<PositionDefinition> positions,
            IReadOnlyList<string> residentNames)
        {
            Traits = traits;
            Positions = positions;
            ResidentNames = residentNames;
        }

        public PositionDefinition FindPosition(string id)
        {
            foreach (PositionDefinition pos in Positions)
            {
                if(pos.Id == id)
                    return pos;
            }
            return null;
        }

        public TraitDefinition FindTrait(string id)
        {
            foreach (TraitDefinition trait in Traits)
            {
                if (trait.Id == id)
                    return trait;
            }
            return null;
        }

        public static GameContent CreatePrototype()
        {
            var traits = new TraitDefinition[]
            {
                new TraitDefinition("farmer", "Rolnik", TraitDomain.Production, foodProductionPercent: 25),
                new TraitDefinition("waterbearer", "Wodonosz", TraitDomain.Production, waterProductionPercent: 25),
                new TraitDefinition("handyman", "Majsterkowicz", TraitDomain.Construction),
                new TraitDefinition("frugal", "Skromny", TraitDomain.Survival, consumptionPercent: -25),
                new TraitDefinition("tough", "Twardziel", TraitDomain.Survival),
                new TraitDefinition("tracker", "Tropiciel", TraitDomain.Survival)
            };
            
            var positions = new PositionDefinition[]
            {
                new PositionDefinition("farm", "Uprawa", ResourceType.Food, 6),
                new PositionDefinition("well", "Studnia", ResourceType.Water, 6)
            };
            
            var names = new string[]
            {
                "Anna", "Marek", "Ola", "Piotr", "Kasia", "Tomek",
                "Zosia", "Janek", "Ewa", "Bartek", "Magda", "Staszek"
            };
            
            return new GameContent(traits, positions, names);
        }
    }
}