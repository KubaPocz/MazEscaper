using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Rola: tworzy początkową grupę mieszkańców (imiona, cechy) z regułą trzech dziedzin (GDD 5.7).
    /// Wejście: liczba osób, treść gry, generator losowości.
    /// Zmienia: nic poza stanem generatora losowości.
    /// Używane przez: GameState.
    /// </summary>
    public static class ResidentGeneratorSystem
    {
        public static List<Resident> Generate(int count, GameContent content, IRandom random)
        {
            List<string> names = PickNames(count,content,random);
            List<TraitDefinition> traits = PickTraits(count, content, random);
            
            var residents = new List<Resident>();
            for(int i =0; i < count; i++)
                residents.Add(new Resident(i,names[i],traits[i]));
            return residents;
        }

        private static List<string> PickNames(int count, GameContent content, IRandom random)
        {
            var pool = new List<string>(content.ResidentNames);
            Shuffle(pool, random);
            var names = new List<string>();
            for (int i = 0; i < count; i++)
                names.Add(i < pool.Count ? pool[i] : "Osadnik" + (i + 1));
            return names;
        }

        private static List<TraitDefinition> PickTraits(int count, GameContent content, IRandom random)
        {
            var result = new List<TraitDefinition>();

            if (content.Traits.Count == 0)
            {
                for (int i = 0; i < count; i++)
                    result.Add(null);
                return result;
            }

            var domains = new List<TraitDomain>();
            foreach (TraitDefinition trait in content.Traits)
            {
                if (!domains.Contains(trait.Domain))
                    domains.Add(trait.Domain);
            }

            Shuffle(domains, random);

            // Jedna cecha z każdej dziedziny (dopóki starcza osób), reszta losowo z całej puli.
            for (int i = 0; i < domains.Count && result.Count < count; i++)
                result.Add(RandomFromDomain(content, domains[i], random));

            while (result.Count < count)
                result.Add(content.Traits[random.NextInt(0, content.Traits.Count)]);

            Shuffle(result, random);
            return result;
        }

        private static TraitDefinition RandomFromDomain(GameContent content, TraitDomain domain, IRandom random)
        {
            var candidates = new List<TraitDefinition>();
            foreach (TraitDefinition trait in content.Traits)
            {
                if (trait.Domain == domain)
                    candidates.Add(trait);
            }

            return candidates[random.NextInt(0, candidates.Count)];
        }

        private static void Shuffle<T>(List<T> list, IRandom random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.NextInt(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}