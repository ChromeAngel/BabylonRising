using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Law : MonoBehaviour {

    public enum faction
    {
        Neutral,
        Human,
        Empire,
        Reptile,
        Bonehead,
        Green,
        Raiders,
        Pirate
    };

    public enum relation
    {
        Hatred, /* Will go out of their way to attack you */
        Competitor, /* Will attack if their interests conflict with yours */
        Neutral, /* will fight back if attacked, but will otherwise ignore you */
        NonAggression, /* Will aid you if it does not conflict with their interests */
        Ally /* Will go out of their way to aid each other */
    }

    public Dictionary<faction, Dictionary<faction, relation>> relations;

    /// <summary>
    /// How does Red Faction relate to Blue Faction
    /// </summary>
    /// <param name="red">one of the factions</param>
    /// <param name="blue">another of the factions</param>
    /// <returns>the relationship between Red and Blue factions</returns>
    public relation getRelation(faction red, faction blue)
    {
        if (red == blue) return relation.Ally; //always an ally of your own faction 

        if (relations == null) return relation.Neutral;

        if(relations.ContainsKey(red))
        {
            if(relations[red].ContainsKey(blue))
            {
                return relations[red][blue];
            } else
            {
                return relation.Neutral;
            }
        } else
        {
            if (relations.ContainsKey(blue))
            {
                if (relations[blue].ContainsKey(red))
                {
                    return relations[blue][red];
                }
                else
                {
                    return relation.Neutral;
                }
            } else
            {
                return relation.Neutral;
            }
        }
    } //end getRelation

    public bool areEnemies(faction red, faction blue)
    {
        return getRelation(red, blue) < relation.Neutral;
    }

    public bool areAllies(faction red, faction blue)
    {
        return getRelation(red, blue) > relation.Neutral;
    }

    public bool areNeutral(faction red, faction blue)
    {
        return getRelation(red, blue) == relation.Neutral;
    }

    private static Law _singleton;

    public void Awake()
    {
        if(_singleton == null)
        {
            _singleton = this;
            relations = new Dictionary<faction, Dictionary<faction, relation>>();
            relations.Add(faction.Human, new Dictionary<faction, relation>());
            relations[faction.Human][faction.Bonehead] = relation.Hatred;
        } else
        {
            Destroy(this);
        }
    }

    private static Law instance()
    {
        if (_singleton == null)
        {
            var go = new GameObject("Law");
            _singleton = go.AddComponent<Law>();
        }

        return _singleton;
    }

    public static relation GetRelation(faction red, faction blue)
    {
        return instance().getRelation(red, blue);
    } //end getRelation

    public static bool AreEnemies(faction red, faction blue)
    {
        return instance().areEnemies(red, blue);
    }

    public static bool AreAllies(faction red, faction blue)
    {
        return instance().areAllies(red, blue);
    }

    public static bool AreNeutral(faction red, faction blue)
    {
        return instance().areNeutral(red, blue);
    }
}
