using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Star Base Module Catalog")]
public class StarbaseModuleCatalog : ScriptableObject {
    public GameObject[] Hubs;
    public GameObject[] Branches;
    public GameObject[] Leaves;
}
