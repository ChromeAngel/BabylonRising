using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class SoundOnImpact : MonoBehaviour {
    public AudioSourceSet audioSet;

    private void Awake()
    {
        if(audioSet == null)
        {
            Debug.LogWarningFormat("SoundOnImpact as no audioSet.  Disabling");

            enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSet.source.volume = 0.3f + (0.7f * (Mathf.Clamp(collision.impulse.magnitude, 0f, 100f) / 100f));

        Debug.LogFormat("{0} has collided with {1} volume {2}", this.PathID(), collision.gameObject.PathID(), audioSet.source.volume);

        audioSet.PlayOne();
    }
}
