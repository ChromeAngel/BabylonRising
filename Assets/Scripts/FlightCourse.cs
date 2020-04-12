using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlightCourse : MonoBehaviour {
    public GameObject WayPoint;
    public int WayPointsRemaining = 10;
    public float radius = 300f;

    private Chance chance;

    public UnityEvent OnWaypointTagged;
    public UnityEvent OnQuestFinished;

    // Use this for initialization
    void Start () {
        chance = GetComponent<Chance>();

        WayPoint.transform.position = transform.position + chance.Vector(radius, -radius); ;
        WayPoint.GetComponent<Rigidbody>().AddTorque(chance.Vector(15f, -15f), ForceMode.Impulse);
        WayPoint.GetComponent<Collectable>().OnCollected += WaypointTagged;
    }

    public void WaypointTagged(GameObject waypoint)
    {
        Debug.Log("Ding! Waypoint tagged");
        
        WayPointsRemaining--;

        if (WayPointsRemaining > 0)
        {
            Vector3 far = WayPoint.transform.position; 

            while(Vector3.Distance(far, WayPoint.transform.position) < 0.5f * radius)
            {
                far = transform.position + chance.Vector(radius, -radius);
            }

            WayPoint.transform.position = far;

            if (OnWaypointTagged != null) OnWaypointTagged.Invoke();
        } else
        {
            WayPoint.gameObject.SetActive(false);
            Debug.Log("Ding! Mission complete");
            if (OnWaypointTagged != null) OnWaypointTagged.Invoke();
            if (OnQuestFinished != null) OnQuestFinished.Invoke();
        }

    }
}
