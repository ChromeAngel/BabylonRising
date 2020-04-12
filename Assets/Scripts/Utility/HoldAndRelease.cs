using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoldAndRelease : MonoBehaviour {

    [System.Serializable]
    public class CallBackEvent : UnityEngine.Events.UnityEvent<string, float> { }

    public string buttonName;

    public CallBackEvent OnRelease;

    private bool isHeld;
    private float holdStartTime;

	// Use this for initialization
	void Start () {
        isHeld = false;
        holdStartTime = float.PositiveInfinity;
	}
	
	// Update is called once per frame
	void Update () {
        bool state = Input.GetButton(buttonName);

        if (state == isHeld) return;
        
        if(isHeld)
        {
            //releaseing

            if(OnRelease != null)
            {
                OnRelease.Invoke(buttonName, Time.time - holdStartTime);
            }

            isHeld = false;
        } else
        {
            //holding
            holdStartTime = Time.time;
            isHeld = true;
        }
	}
}
