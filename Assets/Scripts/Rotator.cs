using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public Vector3 Rate;
    public bool RotateAsChild = true;

	//// Use this for initialization
	//void Start () {
		
	//}
	
	// Update is called once per frame
	void Update () {
        if(!RotateAsChild && transform.parent != null)
        {
            enabled = false;
        }

        Vector3 step = Rate * Time.deltaTime;
        transform.Rotate(step);
	}
}
