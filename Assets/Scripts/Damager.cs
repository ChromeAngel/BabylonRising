using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Damager : MonoBehaviour {

    public float Strength;
    public bool DestroyOnImpact;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null) return;

        Damageable other = collision.collider.GetComponent<Damageable>();

        if (other == null)
        {
        } else
        {
            other.Damage(Strength);
        }

        if(DestroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}
