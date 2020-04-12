using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class StarBaseModule : MonoBehaviour {
    public enum eModuleType
    {
        Root,
        Branch,
        Leaf
    }

    public eModuleType ModuleType;
    public bool isScalable = true;

    public string[] moduleTags;

    //public bool IsBlueprint = true;
    //public bool IsConflicted = false;

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.LogFormat("{0} was collided with", gameObject.name);

    //    if (IsBlueprint == false) return;
    //    if (IsConnected(collision.collider.gameObject)) return;
    //    IsConflicted = true;

    //    Debug.LogFormat("{0} is conflicted", gameObject.name);
    //}


    //private bool IsConnected(GameObject other)
    //{
    //    Transform t = transform;

    //    if (t.childCount == 0) return false;
    //    //var connectors = GetComponentsInChildren<StarBaseConnector>(); //we only want connectors within the immediate children
    //    var connectors = new List<StarBaseConnector>();
        
    //    for(int i=0; i < t.childCount; i++)
    //    {
    //        var c = t.GetChild(i).GetComponent<StarBaseConnector>();

    //        if (c.ConnectedTo == other) return true;
    //    }

    //    return false;
    //}
}
