using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combattent : MonoBehaviour {
    public Law.faction Faction;

    public bool IsEnemy(Combattent other)
    {
        return Law.AreEnemies(Faction, other.Faction);
    }

    public bool IsAlly(Combattent other)
    {
        return Law.AreAllies(Faction, other.Faction);
    }

    public bool IsNeutral(Combattent other)
    {
        return Law.AreNeutral(Faction, other.Faction);
    }

    public Law.relation GetRelation(Combattent other)
    {
        return Law.GetRelation(Faction, other.Faction);
    }
}
