using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StarBaseConnector : MonoBehaviour {
    [Range(0,1)]
    public float RootRatio;
    [Range(0, 1)]
    public float BranchRatio;
    [Range(0, 1)]
    public float LeafRatio;
    [Range(0, 6)]
    public int TypeSet;

    public float MinScale = 1f;
    public float MaxScale = 1f;

    public GameObject ConnectedTo;

    public void Start()
    {
        //try and ensure all the ratios add up to 1
        float total = RootRatio + BranchRatio + LeafRatio;
        float fixRatio = 1f / total;
        RootRatio = RootRatio * fixRatio;
        BranchRatio = BranchRatio * fixRatio;
        LeafRatio = LeafRatio * fixRatio;
    }
}
