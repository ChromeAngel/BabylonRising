using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To give the impression of speed in space we fire particles at the player's camera, faster particles at higher speed
/// </summary>
/// <remarks>
/// I wanted the direction of the particles to match the player's velocity vector, but couldn't get it working quite right
/// </remarks>
[RequireComponent(typeof(Rigidbody))]
public class DustController : MonoBehaviour {

    public ParticleSystem spaceDust;
    public Transform dustPivot;

    private Rigidbody body;
    private ParticleSystem.MainModule speedControl;
    private ParticleSystem.EmissionModule rateControl;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        speedControl = spaceDust.main;
        rateControl = spaceDust.emission;
    }

    // Update is called once per frame
    void Update () {
        // dustPivot.rotation = Quaternion.Euler(body.velocity.normalized);
        //dustPivot.rotation = Quaternion.Euler(-1f * body.velocity);
        speedControl.startSpeed = body.velocity.magnitude * 0.3f;
        rateControl.rateOverTime = body.velocity.magnitude * 1.1f;
    }
}
