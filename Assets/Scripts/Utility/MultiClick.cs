using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Monitor for single, double and tripple presses of a specified button
/// </summary>
public class MultiClick : MonoBehaviour {

    [System.Serializable]
    public class CallBackEvent : UnityEngine.Events.UnityEvent<string> { }

    public string buttonName;
    public float deadTime;

    public CallBackEvent OnClick;
    public CallBackEvent OnDoubleClick;
    public CallBackEvent OnTrippleClick;

    private bool doubleLClicked;
    private bool isHeld;
    //private float holdStartTime;
    private float holdFinishTime;

    private void Start()
    {
        doubleLClicked = false;
        isHeld = false;
       // holdStartTime = float.PositiveInfinity;
        holdFinishTime = float.PositiveInfinity;
    } 

    // Update is called once per frame
    void Update () {
        bool held = Input.GetButton(buttonName);

        //Timeout a tripple click
        if( isHeld == false && (Time.time > (holdFinishTime + deadTime)) )
        {
            doubleLClicked = false;
            holdFinishTime = float.PositiveInfinity;
        }

        if (held == isHeld) return;

        //Button state has changed

        if(held)
        {
           // Debug.Log(buttonName + " held");

            isHeld = true;
         //   holdStartTime = Time.time;
        } else
        {
            //button released
           // Debug.Log(buttonName + " released");

            if (holdFinishTime == float.PositiveInfinity)
            {
                //OnCLick
                //Debug.Log(buttonName + " clicked");
                if (OnClick != null) OnClick.Invoke(buttonName);
            } else
            {
                if (doubleLClicked)
                {
                    //OnTrippleClick
                    //Debug.Log(buttonName + "tripple clicked");
                    if (OnTrippleClick != null) OnTrippleClick.Invoke(buttonName);

                    isHeld = false;
                    doubleLClicked = false;
                    holdFinishTime = float.PositiveInfinity;

                    return;
                }
                else
                {
                    //OnDoubleClick
                    //Debug.Log(buttonName + " double clicked");
                    if (OnDoubleClick != null) OnDoubleClick.Invoke(buttonName);
                    doubleLClicked = true;
                }
            }

            isHeld = false;
            holdFinishTime = Time.time;
        }    
	} //end update
}
