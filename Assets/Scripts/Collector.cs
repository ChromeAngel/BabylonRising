using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collector : MonoBehaviour {

    public delegate void OnCollectedEvent(GameObject collectable);

    public OnCollectedEvent OnCollected;

    protected bool CanCollect(GameObject other)
    {
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        Collectable collect = other.GetComponent<Collectable>();

        if (collect == null) return;

        if (!CanCollect(other.gameObject)) return;

        if (collect.OnCollected != null)
        {
            collect.OnCollected.Invoke(gameObject);
        }

        if (OnCollected != null)
        {
            OnCollected.Invoke(other.gameObject);
        }
    }
}
