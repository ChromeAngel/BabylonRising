using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour {

    public delegate void OnCollectedEvent(GameObject collector);

    public OnCollectedEvent OnCollected;
}
