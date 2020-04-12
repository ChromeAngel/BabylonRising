﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionalParts : MonoBehaviour {
    public GameObject[] parts;

	// Use this for initialization
	void Start () {
        parts = Chance.sShuffle<GameObject>(parts);

        int dump = Chance.sRandom(parts.Length);

		for(int i=0;i< dump; i++)
        {
            parts[i].SetActive(false);
            //GameObject.Destroy(parts[i]);
        }
	}
	
}