using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathCorner : MonoBehaviour {
    public GameObject target;
    public UnityEvent OnTargetSwitched;

    private void OnCollisionEnter(Collision collision)
    {
        var ship = collision.gameObject.GetComponent<Ship>();

        if (ship == null) return;

        ship.target = target;

        if (OnTargetSwitched == null) return;

        OnTargetSwitched.Invoke();
    }

    public void SwitchTarget(GameObject newTarget)
    {
        target = newTarget;
    }
}
