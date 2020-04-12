using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManouvreEngine : MonoBehaviour {
    public int manouvre;

    public void manouvreComplete()
    {
        transform.parent.position = transform.position;
        transform.position = Vector3.zero;
        transform.parent.rotation = transform.rotation;
        transform.rotation = Quaternion.identity;
    }
}
